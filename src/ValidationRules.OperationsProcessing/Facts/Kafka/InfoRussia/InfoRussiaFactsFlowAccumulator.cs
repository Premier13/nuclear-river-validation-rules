using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Confluent.Kafka;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsLogging.API;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.InfoRussia
{
    public sealed class InfoRussiaFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<InfoRussiaFactsFlow, KafkaMessageBatch, AggregatableMessage<ICommand>>
    {
        private readonly IEnumerable<string> _appropriateTopics;
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, IInfoRussiaDto> _deserializer = new InfoRussiaDtoDeserializer();
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly SyncEntityNameActor _syncEntityNameActor;
        private readonly IEventLogger _eventLogger;

        private readonly TransactionOptions _transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.Zero
        };

        public InfoRussiaFactsFlowAccumulator(IKafkaSettingsFactory kafkaSettingsFactory, IDataObjectsActorFactory dataObjectsActorFactory, SyncEntityNameActor syncEntityNameActor, IEventLogger eventLogger/*, AmsFactsFlowTelemetryPublisher telemetryPublisher*/)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _syncEntityNameActor = syncEntityNameActor;
            _eventLogger = eventLogger;
            _appropriateTopics = kafkaSettingsFactory.CreateReceiverSettings(InfoRussiaFactsFlow.Instance).TopicPartitionOffsets
                .Select(x => x.Topic).ToHashSet();
        }
        
        protected override AggregatableMessage<ICommand> Process(KafkaMessageBatch batch)
        {
            var filtered = batch.Results
                .Where(x => _appropriateTopics.Contains(x.Topic));

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
            {
                var dtos = _deserializer.Deserialize(filtered).ToList();
                if (dtos.Count != 0)
                {
                    Process(dtos);
                }

                transaction.Complete();
            }
            
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = Array.Empty<ICommand>(),
            };
        }

        private void Process(IReadOnlyCollection<IInfoRussiaDto> dtos)
        {
            var actors = _dataObjectsActorFactory.Create(new []
            {
                typeof(Firm),
                typeof(FirmInactive),
                typeof(FirmAddress),
                typeof(FirmAddressInactive),
                typeof(FirmAddressCategory),
            });
            
            var commands = new[]
            {
                new SyncInMemoryDataObjectCommand(typeof(Firm), dtos),
                new SyncInMemoryDataObjectCommand(typeof(FirmInactive), dtos),
                new SyncInMemoryDataObjectCommand(typeof(FirmAddress), dtos),
                new SyncInMemoryDataObjectCommand(typeof(FirmAddressInactive), dtos),
                new SyncInMemoryDataObjectCommand(typeof(FirmAddressCategory), dtos),
            };

            var eventsCollector = new FactsEventCollector();
            foreach (var actor in actors)
            {
                var events = actor.ExecuteCommands(commands);
                eventsCollector.Add(events);
            }

            _syncEntityNameActor.ExecuteCommands(commands);

            _eventLogger.Log<IEvent>(eventsCollector.Events().Select(x => new FlowEvent(KafkaFactsFlow.Instance, x)).ToList());
        }
    }
}
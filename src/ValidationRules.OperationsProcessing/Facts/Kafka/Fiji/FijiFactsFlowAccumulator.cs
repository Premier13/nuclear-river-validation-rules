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
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Fiji
{
    public sealed class FijiFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FijiFactsFlow, KafkaMessageBatch, AggregatableMessage<ICommand>>
    {
        private readonly IEnumerable<string> _appropriateTopics;
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, IFijiDto> _deserializer = new FijiDtoDeserializer();
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventLogger _eventLogger;
        
        private readonly TransactionOptions _transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.Zero
        };

        public FijiFactsFlowAccumulator(IKafkaSettingsFactory kafkaSettingsFactory, IDataObjectsActorFactory dataObjectsActorFactory, IEventLogger eventLogger)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventLogger = eventLogger;
            _appropriateTopics = kafkaSettingsFactory.CreateReceiverSettings(FijiFactsFlow.Instance).TopicPartitionOffsets
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
        
        private void Process(IReadOnlyCollection<IFijiDto> dtos)
        {
            var actors = _dataObjectsActorFactory.Create(new []
            {
                typeof(Building),
                typeof(BuildingBulkDelete),
            });
            
            var commands = new ICommand[]
            {
                new SyncInMemoryDataObjectCommand(typeof(Building), dtos),
                new DeleteInMemoryDataObjectCommand(typeof(BuildingBulkDelete), dtos)
            };

            var eventsCollector = new FactsEventCollector();
            foreach (var actor in actors)
            {
                var events = actor.ExecuteCommands(commands);
                eventsCollector.Add(events);
            }

            _eventLogger.Log<IEvent>(eventsCollector.Events().Select(x => new FlowEvent(KafkaFactsFlow.Instance, x)).ToList());
        }
    }
}
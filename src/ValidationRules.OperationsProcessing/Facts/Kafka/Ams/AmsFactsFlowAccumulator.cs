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
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ams
{
    public sealed class AmsFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<AmsFactsFlow, KafkaMessageBatch, AggregatableMessage<ICommand>>
    {
        private readonly IEnumerable<string> _appropriateTopics;
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, AdvertisementDto> _deserializer = new AdvertisementDtoDeserializer();
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly SyncEntityNameActor _syncEntityNameActor;
        private readonly AmsFactsFlowTelemetryPublisher _telemetryPublisher;
        private readonly IEventLogger _eventLogger;

        private readonly TransactionOptions _transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.Zero
        };

        public AmsFactsFlowAccumulator(IKafkaSettingsFactory kafkaSettingsFactory, IDataObjectsActorFactory dataObjectsActorFactory, SyncEntityNameActor syncEntityNameActor, IEventLogger eventLogger, AmsFactsFlowTelemetryPublisher telemetryPublisher)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _syncEntityNameActor = syncEntityNameActor;
            _eventLogger = eventLogger;
            _telemetryPublisher = telemetryPublisher;
            _appropriateTopics = kafkaSettingsFactory.CreateReceiverSettings(AmsFactsFlow.Instance).TopicPartitionOffsets
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

                ProcessAmsStates(batch.Results);

                transaction.Complete();
            }

            _telemetryPublisher.Completed(batch.Results.Count);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = Array.Empty<ICommand>(),
            };
        }

        private void Process(IReadOnlyCollection<AdvertisementDto> dtos)
        {
            var actors = _dataObjectsActorFactory.Create(new []
            {
                typeof(Advertisement)
            });
            
            var commands = new[]
            {
                new SyncInMemoryDataObjectCommand(typeof(Advertisement), dtos)
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

        private void ProcessAmsStates(IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> results)
        {
            if (results.Count == 0)
            {
                return;
            }

            var eldestEventTime = results.Min(x => x.Timestamp.UtcDateTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _telemetryPublisher.Delay((int)delta.TotalMilliseconds);

            var maxOffsetResult = results.Aggregate((a,b) => a.Offset > b.Offset ? a : b);

            _eventLogger.Log(new IEvent[]
            {
                new FlowEvent(KafkaFactsFlow.Instance,
                    new AmsStateIncrementedEvent(
                        new AmsState(maxOffsetResult.Offset, maxOffsetResult.Timestamp.UtcDateTime)
                    )
                ),
                new FlowEvent(KafkaFactsFlow.Instance,
                    new DelayLoggedEvent(DateTime.UtcNow)
                )
            });
        }
    }
}

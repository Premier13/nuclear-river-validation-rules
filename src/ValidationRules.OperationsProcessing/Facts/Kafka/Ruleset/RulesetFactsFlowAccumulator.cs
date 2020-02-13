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
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ruleset
{
    public sealed class RulesetFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<RulesetFactsFlow, KafkaMessageBatch, AggregatableMessage<ICommand>>
    {
        private readonly IEnumerable<string> _appropriateTopics;
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, RulesetDto> _deserializer = new RulesetDtoDeserializer();
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly RulesetFactsFlowTelemetryPublisher _telemetryPublisher;
        private readonly IEventLogger _eventLogger;

        private readonly TransactionOptions _transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.Zero
        };

        public RulesetFactsFlowAccumulator(IKafkaSettingsFactory kafkaSettingsFactory, IDataObjectsActorFactory dataObjectsActorFactory, IEventLogger eventLogger, RulesetFactsFlowTelemetryPublisher telemetryPublisher)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventLogger = eventLogger;
            _telemetryPublisher = telemetryPublisher;
            _appropriateTopics = kafkaSettingsFactory.CreateReceiverSettings(RulesetFactsFlow.Instance).Topics;
        }

        protected override AggregatableMessage<ICommand> Process(KafkaMessageBatch batch)
        {
            var filtered = batch.Results
                .Where(x => _appropriateTopics.Contains(x.Topic));

            var dtos = _deserializer.Deserialize(filtered).ToList();
            if (dtos.Count != 0)
            {
                Process(dtos);
            }

            _telemetryPublisher.Completed(batch.Results.Count);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = Array.Empty<ICommand>(),
            };
        }

        private void Process(IReadOnlyCollection<RulesetDto> dtos)
        {
            using var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions);

            var commands = new[]
            {
                new ReplaceDataObjectCommand(typeof(Storage.Model.Facts.Ruleset), dtos),
                new ReplaceDataObjectCommand(typeof(Storage.Model.Facts.Ruleset.AssociatedRule), dtos),
                new ReplaceDataObjectCommand(typeof(Storage.Model.Facts.Ruleset.DeniedRule), dtos),
                new ReplaceDataObjectCommand(typeof(Storage.Model.Facts.Ruleset.QuantitativeRule), dtos),
                new ReplaceDataObjectCommand(typeof(Storage.Model.Facts.Ruleset.RulesetProject), dtos),
            };
            var dataObjectTypes = commands.Select(x => x.DataObjectType).ToHashSet();

            var actors = _dataObjectsActorFactory.Create(dataObjectTypes);
            var eventsCollector = new FactsEventCollector();
            foreach (var actor in actors)
            {
                var events = actor.ExecuteCommands(commands);
                eventsCollector.Add(events);
            }

            _eventLogger.Log<IEvent>(eventsCollector.Events().Select(x => new FlowEvent(KafkaFactsFlow.Instance, x)).ToList());

            transaction.Complete();
        }
    }
}

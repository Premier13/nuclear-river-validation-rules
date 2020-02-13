using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using NuClear.Replication.Core;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ruleset;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka.Rulesets
{
    public sealed class RulesetFactsBulkCommandFactory : IBulkCommandFactory<ConsumeResult<Ignore, byte[]>>
    {
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, RulesetDto> _deserializer;
        private readonly IEnumerable<string> _appropriateTopics;

        public RulesetFactsBulkCommandFactory(IKafkaSettingsFactory kafkaSettingsFactory)
        {
            _appropriateTopics = kafkaSettingsFactory.CreateReceiverSettings(RulesetFactsFlow.Instance).Topics;
            _deserializer = new RulesetDtoDeserializer();
        }

        public IReadOnlyCollection<ICommand> CreateCommands(IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> messages)
        {
            var filtered = messages.Where(x => _appropriateTopics.Contains(x.Topic));

            var deserializedDtos = _deserializer.Deserialize(filtered).ToList();
            if (deserializedDtos.Count == 0)
            {
                return Array.Empty<ICommand>();
            }

            return DataObjectTypesProvider.RulesetFactTypes
                                                 .Select(factType => new BulkInsertInMemoryDataObjectsCommand(factType, deserializedDtos))
                                                 .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using NuClear.Replication.Core;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ams;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Fiji;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.InfoRussia;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ruleset;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public sealed class KafkaFactsBulkCommandFactory : IBulkCommandFactory<ConsumeResult<Ignore, byte[]>>
    {
        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, object> _amsDeserializer ;
        private readonly IEnumerable<string> _amsTopics;

        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, object> _rulesetDeserializer ;
        private readonly IEnumerable<string> _rulesetTopics;

        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, object> _infoRussiaDeserializer ;
        private readonly IEnumerable<string> _infoRussiaTopics;

        private readonly IDeserializer<ConsumeResult<Ignore, byte[]>, object> _fijiDeserializer ;
        private readonly IEnumerable<string> _fijiTopics;

        public KafkaFactsBulkCommandFactory(IKafkaSettingsFactory kafkaSettingsFactory)
        {
            _amsTopics = kafkaSettingsFactory.CreateReceiverSettings(AmsFactsFlow.Instance).TopicPartitionOffsets
                .Select(x => x.Topic).ToHashSet();
            _amsDeserializer = new AdvertisementDtoDeserializer();

            _rulesetTopics = kafkaSettingsFactory.CreateReceiverSettings(RulesetFactsFlow.Instance).TopicPartitionOffsets
                .Select(x => x.Topic).ToHashSet();
            _rulesetDeserializer = new RulesetDtoDeserializer();

            _infoRussiaTopics = kafkaSettingsFactory.CreateReceiverSettings(InfoRussiaFactsFlow.Instance).TopicPartitionOffsets
                .Select(x => x.Topic).ToHashSet();
            _infoRussiaDeserializer = new InfoRussiaDtoDeserializer();

            _fijiTopics = kafkaSettingsFactory.CreateReceiverSettings(FijiFactsFlow.Instance).TopicPartitionOffsets
                .Select(x => x.Topic).ToHashSet();
            _fijiDeserializer = new FijiDtoDeserializer();
        }
        
        public IReadOnlyCollection<ICommand> CreateCommands(IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> messages) =>
            messages
                .GroupBy(x => x.Topic)
                .SelectMany(x =>
                {
                    if (_amsTopics.Contains(x.Key))
                    {
                        var dtos = _amsDeserializer.Deserialize(x).ToList();
                        if (dtos.Count != 0)
                        {
                            return DataObjectTypesProvider.AmsFactTypes
                                .Select(factType => (ICommand) new BulkInsertInMemoryDataObjectsCommand(factType, dtos));
                        }
                    } else if (_rulesetTopics.Contains(x.Key))
                    {
                        var dtos = _rulesetDeserializer.Deserialize(x).ToList();
                        if (dtos.Count != 0)
                        {
                            return DataObjectTypesProvider.RulesetFactTypes
                                .Select(factType => (ICommand) new BulkInsertInMemoryDataObjectsCommand(factType, dtos));
                        }
                    } else if (_infoRussiaTopics.Contains(x.Key))
                    {
                        var dtos = _infoRussiaDeserializer.Deserialize(x).ToList();
                        if (dtos.Count != 0)
                        {
                            return DataObjectTypesProvider.InfoRussiaFactTypes
                                .Select(factType => (ICommand) new BulkInsertInMemoryDataObjectsCommand(factType, dtos));
                        }
                    } else if (_fijiTopics.Contains(x.Key))
                    {
                        var dtos = _fijiDeserializer.Deserialize(x).ToList();
                        if (dtos.Count != 0)
                        {
                            return DataObjectTypesProvider.FijiFactTypes
                                .Select(factType => (ICommand) new BulkInsertInMemoryDataObjectsCommand(factType, dtos));
                        }
                    } else
                        throw new NotSupportedException($"Unsupported topic {x.Key}");

                    return Array.Empty<ICommand>();
                }).ToList();
    }
}

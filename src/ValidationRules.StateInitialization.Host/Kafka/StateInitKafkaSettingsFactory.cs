using System.Linq;
using Confluent.Kafka;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public sealed class StateInitKafkaSettingsFactory : IKafkaSettingsFactory
    {
        private readonly IKafkaSettingsFactory _wrap;

        public StateInitKafkaSettingsFactory(IKafkaSettingsFactory wrap)
        {
            _wrap = wrap;
        }

        public KafkaAdminSettings CreateAdminSettings() => _wrap.CreateAdminSettings();

        public KafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            var settings = _wrap.CreateReceiverSettings(messageFlow);
            
            // stateinit всегда начинает читать все partitions с Offset.Beginning
            settings.TopicPartitionOffsets = settings.TopicPartitionOffsets
                .Select(x => new TopicPartitionOffset(x.Topic, Partition.Any, Offset.Beginning));
            
            // хак для InfoRussia, интересные нам данные начинаются только с определённого offset
            if (messageFlow.Equals(InfoRussiaFactsFlow.Instance))
            {
                const long InfoRussiaOffset = 7_000_000;
                
                settings.TopicPartitionOffsets = settings.TopicPartitionOffsets.Select(x => new
                    TopicPartitionOffset(x.Topic, x.Partition, InfoRussiaOffset));
            }
            
            return settings;
        }
    }
}
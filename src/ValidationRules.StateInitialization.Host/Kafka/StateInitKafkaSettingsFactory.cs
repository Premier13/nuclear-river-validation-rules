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
            
            // stateinit всегда начинает читать с Offset.Beginning
            settings.Offset = Offset.Beginning;

            return settings;
        }
    }
}
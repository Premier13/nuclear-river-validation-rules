using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Telemetry.Probing;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class BatchingKafkaReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public BatchingKafkaReceiverTelemetryDecorator(KafkaReceiver receiver)
        {
            _receiver = receiver;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek messages from Kafka"))
            {
                var messages = _receiver.Peek();
                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Kafka messages"))
            {
                var succeeded = successfullyProcessedMessages.Cast<KafkaMessageBatch>().ToList();
                var failed = failedProcessedMessages.Cast<KafkaMessageBatch>().ToList();

                _receiver.Complete(succeeded, failed);
            }
        }
    }
}

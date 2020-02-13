using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.Transports.Kafka;
using NuClear.OperationsProcessing.API.Primary;

namespace NuClear.OperationsProcessing.Transports.Kafka
{
    // TODO: перенести в репозиторий operations
    public sealed class KafkaMessageBatch : MessageBase
    {
        public KafkaMessageBatch(IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> results)
        {
            Id = Guid.NewGuid();
            Results = results;
        }

        public override Guid Id { get; }
        public IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> Results { get; }
    }
    
    public sealed class KafkaReceiver : MessageReceiverBase<KafkaMessageBatch, IPerformedOperationsReceiverSettings>
    {
        private readonly IKafkaMessageFlowReceiver _messageFlowReceiver;

        public KafkaReceiver(
            MessageFlowMetadata sourceFlowMetadata,
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IKafkaMessageFlowReceiverFactory messageFlowReceiverFactory)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _messageFlowReceiver = messageFlowReceiverFactory.Create(SourceFlowMetadata.MessageFlow);
        }

        protected override IReadOnlyList<KafkaMessageBatch> Peek()
        {
            var batch = _messageFlowReceiver.ReceiveBatch(MessageReceiverSettings.BatchSize);
            return new[] {new KafkaMessageBatch(batch)};
        }

        protected override void Complete(IEnumerable<KafkaMessageBatch> successfullyProcessedMessages, IEnumerable<KafkaMessageBatch> failedProcessedMessages)
        {
            if (failedProcessedMessages.Any())
            {
                throw new ArgumentException("Kafka processing stopped, some messages cannot be processed");
            }

            _messageFlowReceiver.CompleteBatch(successfullyProcessedMessages.SelectMany(x => x.Results));
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _messageFlowReceiver.Dispose();
            }

            base.OnDispose(disposing);
        }
    }
}
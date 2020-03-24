using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.StateInitialization.Core.Commands;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public sealed class KafkaReplicationCommand : ICommand
    {
        public KafkaReplicationCommand(IMessageFlow messageFlow, ReplicateInBulkCommand replicateInBulkCommand, int batchSize)
        {
            MessageFlow = messageFlow;
            ReplicateInBulkCommand = replicateInBulkCommand;
            BatchSize = batchSize;
        }

        public IMessageFlow MessageFlow { get; }
        public ReplicateInBulkCommand ReplicateInBulkCommand { get; }

        public int BatchSize { get; }
    }
}

using System;
using System.Collections.Generic;
using NuClear.Messaging.API.Flows;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.OperationsProcessing.Facts.Erm;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class SqlEventReceiverConfiguration
    {
        private static readonly IMessageFlow[] AggregatesFlowConsumeFlows =
            {ErmFactsFlow.Instance, KafkaFactsFlow.Instance,};

        private static readonly IMessageFlow[] MessagesFlowConsumeFlows =
            {AggregatesFlow.AggregatesFlow.Instance};

        public IEnumerable<IMessageFlow> GetConsumableFlows(IMessageFlow flow) =>
            flow switch
            {
                AggregatesFlow.AggregatesFlow _ => AggregatesFlowConsumeFlows,
                MessagesFlow.MessagesFlow _ => MessagesFlowConsumeFlows,
                _ => throw new ArgumentException($"Flow '{flow.GetType().Name}' has no configured consumed flows.")
            };
    }
}

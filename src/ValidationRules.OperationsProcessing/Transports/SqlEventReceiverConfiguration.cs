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

        public IEnumerable<IMessageFlow> GetConsumableFlows(IMessageFlow flow)
        {
            switch (flow)
            {
                case AggregatesFlow.AggregatesFlow _:
                    return AggregatesFlowConsumeFlows;
                case MessagesFlow.MessagesFlow _:
                    return MessagesFlowConsumeFlows;
                default:
                    throw new ArgumentException($"Flow '{flow.GetType().Name}' has no configured consumed flows.");
            }
        }
    }
}

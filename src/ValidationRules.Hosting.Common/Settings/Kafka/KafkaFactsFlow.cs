using System;
using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.Hosting.Common.Settings.Kafka
{
    public sealed class KafkaFactsFlow : MessageFlowBase<KafkaFactsFlow>
    {
        public override Guid Id => new Guid("B46F657A-DA2E-43EC-8D83-5C381C77D2D8");

        public override string Description => nameof(KafkaFactsFlow);
    }
}
using System;
using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.Hosting.Common.Settings.Kafka
{
    public sealed class FijiFactsFlow : MessageFlowBase<FijiFactsFlow>
    {
        public override Guid Id => new Guid("2DE3CBA9-C053-48D5-A622-565326086225");

        public override string Description => nameof(FijiFactsFlow);
    }
}
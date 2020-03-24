using System;
using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.Hosting.Common.Settings.Kafka
{
    public sealed class InfoRussiaFactsFlow : MessageFlowBase<InfoRussiaFactsFlow>
    {
        public override Guid Id => new Guid("9B010307-8967-4769-BD76-F959A37857E3");

        public override string Description => nameof(InfoRussiaFactsFlow);
    }
}
using System;
using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Erm
{
    public sealed class ErmFactsFlow : MessageFlowBase<ErmFactsFlow>
    {
        public override Guid Id => new Guid("213A17BF-2945-4F98-B02F-62235C0A107E");

        public override string Description => nameof(ErmFactsFlow);
    }
}

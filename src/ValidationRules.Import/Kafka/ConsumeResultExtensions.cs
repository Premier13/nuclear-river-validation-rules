using System.Collections;
using Confluent.Kafka;

using CommonFormatAccount = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.Account.Account;
using CommonFormatLegalEntity = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalEntity.LegalEntity;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        public static IEnumerable Transform(this ConsumeResult<Ignore, object> consumeResult) =>
            consumeResult?.Message?.Value switch
            {
                CommonFormatAccount account => Transform(consumeResult, account),
                CommonFormatLegalEntity legalEntity => Transform(consumeResult, legalEntity),
                _ => null,
            };
    }
}

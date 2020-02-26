using System.Collections;
using System.Linq;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;

using CommonFormatAccount = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.Account.Account;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult, CommonFormatAccount account)
        {
            yield return new Account
            {
                Id = account.Code,
                LegalPersonId = account.LegalEntityCode,
                BranchOfficeOrganizationUnitId = account.LegalEntityBranchCode,
                Balance = account.Balance,
            };

            if (account.ChangedOperations != null)
            {
                foreach (var operation in account.ChangedOperations.Where(x => x.OnBasisOf?.OrderPayment != null))
                {
                    yield return new AccountDetail
                    {
                        Id = operation.Code,
                        IsDeleted = operation.IsDeleted,
                        AccountId = account.Code,
                        OrderId = operation.OnBasisOf.OrderPayment.InvoiceCode,
                        PeriodStartDate = operation.OnBasisOf.OrderPayment.StartDate,
                    };
                }
            }

            yield return new ConsumerState
            {
                Topic = consumeResult.Topic,
                Partition = consumeResult.Partition,
                Offset = consumeResult.Offset
            };
        }
    }
}

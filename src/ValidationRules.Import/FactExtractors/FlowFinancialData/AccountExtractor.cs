using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using Account = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.Account.Account;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowFinancialData
{
    public sealed class AccountExtractor : XmlFactExtractorBase<Account>
    {
        protected override IEnumerable<object> Extract(Account account)
        {
            yield return new Model.PersistentFacts.Account
            {
                Id = account.Code,
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
        }
    }
}

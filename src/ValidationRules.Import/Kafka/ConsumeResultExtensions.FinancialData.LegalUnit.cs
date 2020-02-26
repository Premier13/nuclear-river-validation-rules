using System.Collections;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;

using CommonFormatLegalUnit = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalUnit.LegalUnit;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult, CommonFormatLegalUnit legalUnit)
        {
            yield return new BranchOffice
            {
                Id = legalUnit.Code,
                IsDeleted = legalUnit.IsHidden,
            };

            if (legalUnit.LegalEntityBranches != null)
            {
                foreach (var branch in legalUnit.LegalEntityBranches)
                {
                    yield return new BranchOfficeOrganizationUnit
                    {
                        Id = branch.Code,
                        BranchOfficeId = legalUnit.Code,
                        IsDeleted = branch.IsHidden
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

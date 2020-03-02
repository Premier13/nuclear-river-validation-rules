using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalUnit;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Extractors.FlowFinancialData
{
    public sealed class LegalUnitExtractor : XmlFactExtractorBase<LegalUnit>
    {
        protected override IEnumerable<object> Extract(LegalUnit legalUnit)
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
        }
    }
}

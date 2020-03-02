using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.CpcInfo;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Extractors.FlowAdvModelsInfo
{
    public sealed class CpcInfoExtractor : XmlFactExtractorBase<CpcInfo>
    {
        protected override IEnumerable<object> Extract(CpcInfo cpcInfo)
        {
            foreach (var rubric in cpcInfo.Rubrics)
            {
                yield return new CostPerClickCategoryRestriction
                {
                    ProjectId = cpcInfo.BranchCode,
                    Start = cpcInfo.BeginningDate,
                    CategoryId = rubric.Code,
                    MinCostPerClick = (decimal)rubric.Cpc,
                };
            }
        }
    }
}

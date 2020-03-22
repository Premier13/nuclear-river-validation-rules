using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.CpcInfo;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowAdvModelsInfo
{
    public sealed class CpcInfoExtractor : XmlFactExtractorBase<CpcInfo>
    {
        protected override IEnumerable<object> Extract(CpcInfo cpcInfo)
        {
            var key = new CostPerClickCategoryRestriction.GroupKey
                {ProjectId = cpcInfo.BranchCode, Start = cpcInfo.BeginningDate};

            yield return Group.Create(
                key,
                cpcInfo.Rubrics.Select(rubric => new CostPerClickCategoryRestriction
                {
                    ProjectId = cpcInfo.BranchCode,
                    Start = cpcInfo.BeginningDate,
                    CategoryId = rubric.Code,
                    MinCostPerClick = (decimal) rubric.Cpc,
                }).ToList());
        }
    }
}

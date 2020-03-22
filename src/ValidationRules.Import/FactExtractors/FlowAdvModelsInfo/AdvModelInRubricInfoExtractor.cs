using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.AdvModelInRubricInfo;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowAdvModelsInfo
{
    public sealed class AdvModelInRubricInfoExtractor : XmlFactExtractorBase<AdvModelInRubricInfo>
    {
        protected override IEnumerable<object> Extract(AdvModelInRubricInfo advModelInRubricInfo)
        {
            var key = new SalesModelCategoryRestriction.GroupKey
                {ProjectId = advModelInRubricInfo.BranchCode, Start = advModelInRubricInfo.BeginningDate};

            yield return Group.Create(
                key,
                advModelInRubricInfo.AdvModelsInRubrics.Select(rubric =>
                    new SalesModelCategoryRestriction
                    {
                        ProjectId = advModelInRubricInfo.BranchCode,
                        Start = advModelInRubricInfo.BeginningDate,
                        CategoryId = rubric.Code,
                        SalesModel = (int) rubric.AdvModel,
                    }).ToList());
        }
    }
}

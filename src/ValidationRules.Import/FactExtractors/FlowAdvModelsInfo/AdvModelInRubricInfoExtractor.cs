using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.AdvModelInRubricInfo;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowAdvModelsInfo
{
    public sealed class AdvModelInRubricInfoExtractor : XmlFactExtractorBase<AdvModelInRubricInfo>
    {
        protected override IEnumerable<object> Extract(AdvModelInRubricInfo advModelInRubricInfo)
        {
            foreach (var rubric in advModelInRubricInfo.AdvModelsInRubrics)
            {
                yield return new SalesModelCategoryRestriction
                {
                    ProjectId = advModelInRubricInfo.BranchCode,
                    Start = advModelInRubricInfo.BeginningDate,
                    CategoryId = rubric.Code,
                    SalesModel = (int)rubric.AdvModel,
                };
            }
        }
    }
}

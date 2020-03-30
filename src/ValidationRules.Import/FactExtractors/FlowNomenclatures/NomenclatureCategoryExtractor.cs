using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureCategory;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowNomenclatures
{
    public sealed class NomenclatureCategoryExtractor : XmlFactExtractorBase<NomenclatureCategory>
    {
        protected override IEnumerable<object> Extract(NomenclatureCategory nomenclatureCategory)
        {
            yield return new Model.PersistentFacts.EntityName
            {
                Id = nomenclatureCategory.Code,
                EntityType = 285,
                Name = nomenclatureCategory.Name,
            };

            // Храним всё. В том числе удалённые.
            yield return new Model.PersistentFacts.NomenclatureCategory
            {
                Id = nomenclatureCategory.Code,
            };
        }
    }
}

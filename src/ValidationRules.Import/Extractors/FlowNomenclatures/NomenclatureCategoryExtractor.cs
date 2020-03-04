using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureCategory;

namespace NuClear.ValidationRules.Import.Extractors.FlowNomenclatures
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

            yield return new Model.PersistentFacts.NomenclatureCategory
            {
                Id = nomenclatureCategory.Code,
                IsDeleted = nomenclatureCategory.IsDeleted,
            };
        }
    }
}

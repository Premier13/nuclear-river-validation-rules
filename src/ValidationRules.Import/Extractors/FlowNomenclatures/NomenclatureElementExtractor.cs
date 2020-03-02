using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureElement;

namespace NuClear.ValidationRules.Import.Extractors.FlowNomenclatures
{
    public sealed class NomenclatureElementExtractor : XmlFactExtractorBase<NomenclatureElement>
    {
        protected override IEnumerable<object> Extract(NomenclatureElement nomenclatureElement)
        {
            yield return new Model.PersistentFacts.Position
            {
                Id = nomenclatureElement.Code,
                CategoryCode = nomenclatureElement.NomenclatureCategoryCode,
                ContentSales = (int)nomenclatureElement.ContentSales,
                IsDeleted = nomenclatureElement.IsDeleted,
                PositionsGroup = (int)nomenclatureElement.PositionsGroup,
                SalesModel = (int)nomenclatureElement.AdvModel,
                BindingObjectType = (int)nomenclatureElement.LinkObjectType,
                IsCompositionOptional = nomenclatureElement.IsCompositionOptional,
                IsControlledByAmount = nomenclatureElement.IsControlledByAmount,
            };

            if (nomenclatureElement.Composition != null)
            {
                foreach (var simple in nomenclatureElement.Composition)
                {
                    // fixme: это надо или он обязательно уже объявлен должен быть ранее?
                    yield return new Model.PersistentFacts.Position
                    {
                        Id = simple.Code,
                        CategoryCode = simple.NomenclatureCategoryCode,
                        ContentSales = (int)simple.ContentSales,
                        IsDeleted = false,
                        PositionsGroup = (int)simple.PositionsGroup,
                        SalesModel = (int)simple.AdvModel,
                        BindingObjectType = (int)simple.LinkObjectType,
                        IsCompositionOptional = false,
                        IsControlledByAmount = simple.IsControlledByAmount,
                    };

                    yield return new Model.PersistentFacts.PositionChild
                    {
                        MasterPositionId = nomenclatureElement.Code,
                        ChildPositionId = simple.Code,
                    };
                }
            }
        }
    }
}

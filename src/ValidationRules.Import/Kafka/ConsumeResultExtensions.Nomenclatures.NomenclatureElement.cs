using System.Collections;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.Service;

using NomenclatureElement = NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureElement.NomenclatureElement;
using PersistentFacts = NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult,  NomenclatureElement nomenclatureElement)
        {
            yield return new PersistentFacts.Position
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
                    yield return new PersistentFacts.Position
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

                    yield return new PersistentFacts.PositionChild
                    {
                        MasterPositionId = nomenclatureElement.Code,
                        ChildPositionId = simple.Code,
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

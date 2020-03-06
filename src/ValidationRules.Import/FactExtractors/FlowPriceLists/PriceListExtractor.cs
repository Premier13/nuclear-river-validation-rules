using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowPriceLists.PriceList;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowPriceLists
{
    public sealed class PriceListExtractor : XmlFactExtractorBase<PriceList>
    {
        protected override IEnumerable<object> Extract(PriceList priceList)
        {
            yield return new Price
            {
                Id = priceList.Code,
                ProjectId = priceList.BranchCode,
                BeginDate = priceList.BeginingDate,
                IsDeleted = priceList.IsDeleted || !priceList.IsPublished,
            };

            if (priceList.PriceListPositions != null)
            {
                foreach (var position in priceList.PriceListPositions)
                {
                    yield return new PricePosition
                    {
                        Id = position.Code,
                        PriceId = priceList.Code,
                        PositionId = position.NomenclatureCode,
                        IsActiveNotDeleted = !position.IsHidden && !position.IsDeleted,
                    };
                }
            }
        }
    }
}

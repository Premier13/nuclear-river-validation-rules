using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureElement;

namespace NuClear.ValidationRules.Import.Extractors.FlowNomenclatures
{
    public sealed class NomenclatureElementExtractor : XmlFactExtractorBase<NomenclatureElement>
    {
        private static readonly IReadOnlyDictionary<long, int> CountryCodeToTenant = new Dictionary<long, int>
        {
            {1, 1}, // RU
            {18, 2}, // CZ
            {19, 3}, // CY
            {14, 4}, // AE
            {4, 5}, // KZ
            {23, 6}, // KG
            {11, 7}, // UA
            {194, 9}, // UZ
            {16, 10}, // AZ
        };

        private static readonly HashSet<long> SupportedCountryCodes = new HashSet<long>(CountryCodeToTenant.Keys);

        protected override IEnumerable<object> Extract(NomenclatureElement nomenclatureElement)
        {
            foreach (var country in nomenclatureElement.Countries.Where(x => SupportedCountryCodes.Contains(x.CountryCode)))
            {
                yield return new Model.PersistentFacts.EntityName
                {
                    Id = nomenclatureElement.Code,
                    EntityType = 153,
                    Name = country.Localizations.First().Name,
                    TenantId = CountryCodeToTenant[country.CountryCode],
                };
            }

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
                    // yield return new Model.PersistentFacts.Position
                    // {
                    //     Id = simple.Code,
                    //     CategoryCode = simple.NomenclatureCategoryCode,
                    //     ContentSales = (int)simple.ContentSales,
                    //     IsDeleted = nomenclatureElement.IsDeleted,
                    //     PositionsGroup = (int)simple.PositionsGroup,
                    //     SalesModel = (int)simple.AdvModel,
                    //     BindingObjectType = (int)simple.LinkObjectType,
                    //     IsCompositionOptional = false,
                    //     IsControlledByAmount = simple.IsControlledByAmount,
                    // };

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

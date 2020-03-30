using System;
using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureElement;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowNomenclatures
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
            foreach (var country in nomenclatureElement.Countries.Where(x =>
                SupportedCountryCodes.Contains(x.CountryCode)))
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
                ContentSales = ConvertContentSales(nomenclatureElement.ContentSales),
                IsDeleted = nomenclatureElement.IsDeleted,
                PositionsGroup = ConvertPositionsGroup(nomenclatureElement.PositionsGroup),
                SalesModel = ConvertAdvModel(nomenclatureElement.AdvModel),
                BindingObjectType = ConvertLinkObjectType(nomenclatureElement.LinkObjectType),
                IsCompositionOptional = nomenclatureElement.IsCompositionOptional,
                IsControlledByAmount = nomenclatureElement.IsControlledByAmount,
            };

            yield return Group.Create(
                new {MasterPositionId = nomenclatureElement.Code},
                (nomenclatureElement.Composition ?? Array.Empty<NomenclatureElementSimple>())
                .Select(simple => new Model.PersistentFacts.PositionChild
                {
                    MasterPositionId = nomenclatureElement.Code,
                    ChildPositionId = simple.Code,
                }).ToList());
        }

        // todo: строго говоря, нам не обязательно и даже вредно сохранять соответствие кодов с erm.
        // но так проще провалидировать импорт, поэтому на первом этапе будем соответствовать. дальше будет видно.
        private int ConvertContentSales(ContentSales contentSales)
            => contentSales switch
            {
                ContentSales.WithoutContent => 1,
                ContentSales.ContentIsNotRequired => 2,
                ContentSales.ContentIsRequired => 3,
            };

        private int ConvertPositionsGroup(PositionsGroup positionsGroup)
            => positionsGroup switch
            {
                PositionsGroup.None => 0,
                PositionsGroup.MediaAdvertising => 1,
            };

        private int ConvertAdvModel(AdvModel advModel)
            => advModel switch
            {
                AdvModel.CPS => 10,
                AdvModel.FH => 11,
                AdvModel.MFH => 12,
                AdvModel.CPU => 13,
                AdvModel.CPM => 14,
            };

        private int ConvertLinkObjectType(LinkObjectType linkObjectType)
            => linkObjectType switch
            {
                LinkObjectType.Firm => 9,

                LinkObjectType.CategorySingle => 33,
                LinkObjectType.CategoryMultiple => 34,
                LinkObjectType.CategoryMultipleAsterix => 1,

                LinkObjectType.AddressSingle => 6,
                LinkObjectType.AddressMultiple => 35,

                LinkObjectType.AddressCategorySingle => 7,
                LinkObjectType.AddressCategoryMultiple => 8,
                LinkObjectType.AddressFirstLevelCategorySingle => 36,
                LinkObjectType.AddressFirstLevelCategoryMultiple => 37,

                LinkObjectType.ThemeMultiple => 40,
            };
    }
}

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
                ContentSales = (int) nomenclatureElement.ContentSales,
                IsDeleted = nomenclatureElement.IsDeleted,
                PositionsGroup = (int) nomenclatureElement.PositionsGroup,
                SalesModel = (int) nomenclatureElement.AdvModel,
                BindingObjectType = (int) nomenclatureElement.LinkObjectType,
                IsCompositionOptional = nomenclatureElement.IsCompositionOptional,
                IsControlledByAmount = nomenclatureElement.IsControlledByAmount,
            };

            yield return Group.Create(
                new {MasterPositionId = (long) nomenclatureElement.Code},
                (nomenclatureElement.Composition ?? Array.Empty<NomenclatureElementSimple>())
                .Select(simple => new Model.PersistentFacts.PositionChild
                {
                    MasterPositionId = nomenclatureElement.Code,
                    ChildPositionId = simple.Code,
                }).ToList());
        }
    }
}

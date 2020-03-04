using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalEntity;
using NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Extractors.FlowFinancialData
{
    public sealed class LegalEntityExtractor : XmlFactExtractorBase<LegalEntity>
    {
        protected override IEnumerable<object> Extract(LegalEntity legalEntity)
        {
            yield return new LegalPerson
            {
                Id = legalEntity.Code,
                IsDeleted = legalEntity.IsDeleted || legalEntity.IsHidden,
            };

            if (legalEntity.Profiles != null)
            {
                foreach (var profile in legalEntity.Profiles)
                {
                    var x = new LegalPersonProfile
                    {
                        Id = profile.Code,
                        LegalPersonId = legalEntity.Code,
                        IsDeleted = legalEntity.IsDeleted || legalEntity.IsHidden || profile.IsHidden,
                    };

                    switch (ExtractWarrantyOrBargain(profile.Item))
                    {
                        case BargainDetails bargainDetails when bargainDetails.BargainEndDateSpecified:
                            x.BargainEndDate = bargainDetails.BargainEndDate;
                            break;
                        case WarrantyDetails warrantyDetails when warrantyDetails.WarrantyEndDateSpecified:
                            x.WarrantyEndDate = warrantyDetails.WarrantyEndDate;
                            break;
                    }

                    yield return x;

                    yield return new EntityName
                    {
                        Id = profile.Code,
                        EntityType = 219,
                        Name = profile.Name,
                    };
                }
            }

            object ExtractWarrantyOrBargain(object profileExtension)
                => profileExtension switch
                {
                    ProfileExtAE ae => null,
                    ProfileExtAZ az => az.Item,
                    ProfileExtCY cy => cy.Item,
                    ProfileExtCZ cz => cz.Item,
                    ProfileExtKG kg => kg.Item,
                    ProfileExtKZ kz => kz.Item,
                    ProfileExtRU ru => ru.Item,
                    ProfileExtUA ua => ua.Item,
                    ProfileExtUZ uz => uz.Item,
                    null => throw new ArgumentNullException("Missing profile extension.", nameof(profileExtension)),
                    _ => throw new ArgumentException($"Unsupported profile extension type {profileExtension.GetType()}.")
                };
        }
    }
}

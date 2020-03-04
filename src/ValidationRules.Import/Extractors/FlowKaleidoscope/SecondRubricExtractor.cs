using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowKaleidoscope.SecondRubric;
using PersistentFacts = NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Extractors.FlowKaleidoscope
{
    public sealed class SecondRubricExtractor : XmlFactExtractorBase<SecondRubric>
    {
        protected override IEnumerable<object> Extract(SecondRubric secondRubric)
        {
            if (secondRubric.IsDeleted)
            {
                yield return new PersistentFacts.Category {Id = secondRubric.Code, IsDeleted = true};
                yield return Group.Create(
                    new {CategoryId = secondRubric.Code},
                    Array.Empty<PersistentFacts.CategoryProject>());
            }
            else
            {
                foreach (var localization in secondRubric.Localizations)
                {
                    yield return new PersistentFacts.EntityName
                    {
                        Id = secondRubric.Code,
                        TenantId = 0,
                        EntityType = 160,
                        Name = localization.Name,
                        // todo: here and everywhere Lang = localization.Lang,
                    };
                }

                yield return new PersistentFacts.Category
                    {Id = secondRubric.Code, L1Id = secondRubric.CategoryCode, L2Id = secondRubric.CategoryCode};

                yield return Group.Create(
                    new {CategoryId = secondRubric.Code},
                    Array.Empty<PersistentFacts.CategoryProject>());
            }
        }
    }
}

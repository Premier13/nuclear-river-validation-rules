using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Import.Model;
using Category = NuClear.ValidationRules.Import.Model.CommonFormat.flowKaleidoscope.Category.Category;
using PersistentFacts = NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowKaleidoscope
{
    public sealed class CategoryExtractor : XmlFactExtractorBase<Category>
    {
        protected override IEnumerable<object> Extract(Category category)
        {
            if (category.IsDeleted)
            {
                yield return new PersistentFacts.Category {Id = category.Code, IsDeleted = true};
                yield return Group.Create(
                    new {CategoryId = (long) category.Code},
                    Array.Empty<PersistentFacts.CategoryProject>());
            }
            else
            {
                foreach (var localization in category.Localizations)
                {
                    yield return new PersistentFacts.EntityName
                    {
                        Id = category.Code,
                        TenantId = 0,
                        EntityType = 160,
                        Name = localization.Name,
                        // todo: here and everywhere Lang = localization.Lang,
                    };
                }

                yield return new PersistentFacts.Category {Id = category.Code, L1Id = category.Code};
                yield return Group.Create(
                    new {CategoryId = (long) category.Code},
                    Array.Empty<PersistentFacts.CategoryProject>());
            }
        }
    }
}

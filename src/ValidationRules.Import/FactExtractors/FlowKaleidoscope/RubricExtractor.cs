using System;
using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.CommonFormat.flowKaleidoscope.Rubric;
using PersistentFacts = NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.FactExtractors.FlowKaleidoscope
{
    public sealed class RubricExtractor : XmlFactExtractorBase<Rubric>
    {
        protected override IEnumerable<object> Extract(Rubric rubric)
        {
            if (rubric.IsDeleted)
            {
                yield return new PersistentFacts.Category {Id = rubric.Code, IsDeleted = true};
                yield return Group.Create(
                    new {CategoryId = rubric.Code},
                    Array.Empty<PersistentFacts.CategoryProject>());
            }
            else
            {
                foreach (var localization in rubric.Localizations)
                {
                    yield return new PersistentFacts.EntityName
                    {
                        Id = rubric.Code,
                        TenantId = 0,
                        EntityType = 160,
                        Name = localization.Name,
                        // todo: here and everywhere Lang = localization.Lang,
                    };
                }

                yield return new PersistentFacts.Category
                    {Id = rubric.Code, L2Id = rubric.SecondRubricCode, L3Id = rubric.Code};

                yield return Group.Create(
                    new {CategoryId = rubric.Code},
                    rubric.Groups.SelectMany(x => x.Branches ?? Array.Empty<BranchesGroupBranch>())
                        .Select(x => new PersistentFacts.CategoryProject {CategoryId = rubric.Code, ProjectId = x.Code})
                        .ToList());
            }
        }
    }
}

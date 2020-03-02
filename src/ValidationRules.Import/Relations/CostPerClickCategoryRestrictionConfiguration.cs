using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class CostPerClickCategoryRestrictionConfiguration : IEntityConfiguration,
        IRelationProvider<CostPerClickCategoryRestriction>
    {
        public void Apply(Cache cache)
            => cache.Entity<CostPerClickCategoryRestriction>()
                .HasKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<CostPerClickCategoryRestriction>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(Writer writer)
            => writer.Entity<CostPerClickCategoryRestriction>()
                .HasRelationsProvider(this)
                .HasKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<CostPerClickCategoryRestriction> updated, IQueryable<CostPerClickCategoryRestriction> outdated)
        {
            const string costPerClickCategoryRestrictionName =
                "NuClear.ValidationRules.Storage.Model.Facts.CostPerClickCategoryRestriction";
            const string projectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

            var projectRelations =
                from restriction in updated.Union(outdated)
                select new RelationRecord(costPerClickCategoryRestrictionName, projectName, restriction.ProjectId);

            return projectRelations.ToList();
        }
    }
}

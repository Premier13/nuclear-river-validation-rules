using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class SalesModelCategoryRestrictionConfiguration : IEntityConfiguration,
        IRelationProvider<SalesModelCategoryRestriction>
    {
        public void Apply(Cache cache)
            => cache.Entity<SalesModelCategoryRestriction>()
                .HasKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<SalesModelCategoryRestriction>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(Writer writer)
            => writer.Entity<SalesModelCategoryRestriction>()
                .HasRelationsProvider(this)
                .HasKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<SalesModelCategoryRestriction> actual, IQueryable<SalesModelCategoryRestriction> outdated)
        {
            const string salesModelCategoryRestrictionName =
                "NuClear.ValidationRules.Storage.Model.Facts.SalesModelCategoryRestriction";
            const string projectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

            var projectRelations =
                from restriction in actual.Union(outdated)
                select new RelationRecord(salesModelCategoryRestrictionName, projectName, restriction.ProjectId);

            return projectRelations.ToList();
        }
    }
}

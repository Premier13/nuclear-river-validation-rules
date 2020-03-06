using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.FactWriters;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactConfigurations
{
    public sealed class SalesModelCategoryRestrictionConfiguration : IEntityConfiguration,
        IRelationProvider<SalesModelCategoryRestriction>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<SalesModelCategoryRestriction>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<SalesModelCategoryRestriction>()
                .HasRelationsProvider(enableRelations ? this : null)
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

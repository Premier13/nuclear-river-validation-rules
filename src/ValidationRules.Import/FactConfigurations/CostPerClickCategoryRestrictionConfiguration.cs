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
    public sealed class CostPerClickCategoryRestrictionConfiguration : IEntityConfiguration,
        IRelationProvider<CostPerClickCategoryRestriction>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<CostPerClickCategoryRestriction>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<CostPerClickCategoryRestriction>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => new {x.ProjectId, x.Start, x.CategoryId});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<CostPerClickCategoryRestriction> actual, IQueryable<CostPerClickCategoryRestriction> outdated)
        {
            const string costPerClickCategoryRestrictionName =
                "NuClear.ValidationRules.Storage.Model.Facts.CostPerClickCategoryRestriction";
            const string projectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

            var projectRelations =
                from restriction in actual.Union(outdated)
                select new RelationRecord(costPerClickCategoryRestrictionName, projectName, restriction.ProjectId);

            return projectRelations.ToList();
        }
    }
}

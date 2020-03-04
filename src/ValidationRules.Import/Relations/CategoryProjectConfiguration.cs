using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class CategoryProjectConfiguration : IEntityConfiguration, IRelationProvider<CategoryProject>
    {
        public void Apply(FluentMappingBuilder builder) =>
            builder.Entity<CategoryProject>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.CategoryId, x.ProjectId});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<CategoryProject>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasGroupKey(x => new {x.CategoryId});

        public IReadOnlyCollection<RelationRecord> GetRelations(
            DataConnection dataConnection,
            IQueryable<CategoryProject> actual,
            IQueryable<CategoryProject> outdated)
        {
            // Не удивляйся типу, копипаста из оригнала для сохранения события. Нужно изменить тип и обновить метаданные.
            const string categoryOrganizationUnitName
                = "NuClear.ValidationRules.Storage.Model.Facts.CategoryOrganizationUnit";
            const string projectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

            var projectRelations =
                from categoryProject in actual.Union(outdated)
                select new RelationRecord(categoryOrganizationUnitName, projectName, categoryProject.ProjectId);

            return projectRelations.ToList();
        }
    }
}

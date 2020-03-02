using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class BranchOfficeOrganizationUnitConfiguration : IEntityConfiguration,
        IRelationProvider<BranchOfficeOrganizationUnit>
    {
        public void Apply(Cache cache)
            => cache.Entity<BranchOfficeOrganizationUnit>()
                .HasKey(x => x.Id);

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<BranchOfficeOrganizationUnit>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(Writer writer)
            => writer.Entity<BranchOfficeOrganizationUnit>()
                .HasRelationsProvider(this)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<BranchOfficeOrganizationUnit> updated, IQueryable<BranchOfficeOrganizationUnit> outdated)
        {
            const string branchOfficeOrganizationUnitName =
                "NuClear.ValidationRules.Storage.Model.Facts.BranchOfficeOrganizationUnit";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from boou in updated.Union(outdated)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.BranchOfficeOrganizationUnitId == boou.Id)
                select new RelationRecord(branchOfficeOrganizationUnitName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}

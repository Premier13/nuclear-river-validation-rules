using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.FactWriters;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactConfigurations
{
    public sealed class BranchOfficeConfiguration : IEntityConfiguration, IRelationProvider<BranchOffice>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<BranchOffice>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<BranchOffice>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<BranchOffice> actual, IQueryable<BranchOffice> outdated)
        {
            const string branchOfficeName = "NuClear.ValidationRules.Storage.Model.Facts.BranchOffice";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from branchOffice in actual.Union(outdated)
                from boou in dataConnection.GetTable<BranchOfficeOrganizationUnit>()
                    .InnerJoin(x => x.BranchOfficeId == branchOffice.Id)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.BranchOfficeOrganizationUnitId == boou.Id)
                select new RelationRecord(branchOfficeName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}
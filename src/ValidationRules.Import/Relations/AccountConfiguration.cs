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
    public sealed class AccountConfiguration : IEntityConfiguration, IRelationProvider<Account>
    {
        public void Apply(Cache cache)
            => cache.Entity<Account>()
                .HasKey(x => x.Id);

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<Account>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(Writer writer)
            => writer.Entity<Account>()
                .HasRelationsProvider(this)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<Account> updated, IQueryable<Account> outdated)
        {
            const string accountName = "NuClear.ValidationRules.Storage.Model.Facts.Account";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from account in updated.Union(outdated)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.LegalPersonId == account.LegalPersonId &&
                        x.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId)
                select new RelationRecord(accountName, orderName, order.Id);

            var accountRelations =
                from account in updated.Union(outdated)
                select new RelationRecord(accountName, accountName, account.Id);

            var result = new List<RelationRecord>();
            result.AddRange(orderRelations);
            result.AddRange(accountRelations);
            return result;
        }
    }
}

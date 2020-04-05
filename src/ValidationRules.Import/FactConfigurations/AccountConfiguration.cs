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
    public sealed class AccountConfiguration : IEntityConfiguration, IRelationProvider<Account>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<Account>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<Account>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<Account> actual, IQueryable<Account> outdated)
        {
            const string accountName = "NuClear.ValidationRules.Storage.Model.Facts.Account";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from account in actual.Union(outdated)
                from bargain in dataConnection.GetTable<Bargain>()
                    .InnerJoin(x => x.AccountId == account.Id)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.BargainId == bargain.Id)
                select new RelationRecord(accountName, orderName, order.Id);

            var accountRelations =
                from account in actual.Union(outdated)
                select new RelationRecord(accountName, accountName, account.Id);

            var result = new List<RelationRecord>();
            result.AddRange(orderRelations);
            result.AddRange(accountRelations);
            return result;
        }
    }
}

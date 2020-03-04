using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class AccountDetailConfiguration : IEntityConfiguration, IRelationProvider<AccountDetail>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<AccountDetail>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<AccountDetail>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<AccountDetail> actual, IQueryable<AccountDetail> outdated)
        {
            const string accountDetailName = "NuClear.ValidationRules.Storage.Model.Facts.AccountDetail";
            const string accountName = "NuClear.ValidationRules.Storage.Model.Facts.Account";

            var accountRelations =
                from accountDetail in actual.Union(outdated)
                select new RelationRecord(accountDetailName, accountName, accountDetail.AccountId);

            return accountRelations.ToList();
        }
    }
}

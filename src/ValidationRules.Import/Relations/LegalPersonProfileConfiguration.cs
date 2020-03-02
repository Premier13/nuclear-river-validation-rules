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
    public sealed class LegalPersonProfileConfiguration : IEntityConfiguration, IRelationProvider<LegalPersonProfile>
    {
        public void Apply(Cache cache)
            => cache.Entity<LegalPersonProfile>()
                .HasKey(x => x.Id);

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<LegalPersonProfile>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(Writer writer)
            => writer.Entity<LegalPersonProfile>()
                .HasRelationsProvider(this)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<LegalPersonProfile> updated, IQueryable<LegalPersonProfile> outdated)
        {
            const string legalPersonProfileName = "NuClear.ValidationRules.Storage.Model.Facts.LegalPersonProfile";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from legalPersonProfile in updated.Union(outdated)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.LegalPersonId == legalPersonProfile.LegalPersonId)
                select new RelationRecord(legalPersonProfileName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}

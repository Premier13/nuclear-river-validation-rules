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
    public sealed class LegalPersonConfiguration : IEntityConfiguration, IRelationProvider<LegalPerson>
    {
        public void Apply(Cache cache)
            => cache.Entity<LegalPerson>()
                .HasKey(x => x.Id);

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<LegalPerson>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(Writer writer)
            => writer.Entity<LegalPerson>()
                .HasRelationsProvider(this)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<LegalPerson> actual, IQueryable<LegalPerson> outdated)
        {
            const string legalPersonName = "NuClear.ValidationRules.Storage.Model.Facts.LegalPerson";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from legalPerson in actual.Union(outdated)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.LegalPersonId == legalPerson.Id)
                select new RelationRecord(legalPersonName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}

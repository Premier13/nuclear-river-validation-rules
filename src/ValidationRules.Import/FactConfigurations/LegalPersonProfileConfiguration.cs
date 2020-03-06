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
    public sealed class LegalPersonProfileConfiguration : IEntityConfiguration, IRelationProvider<LegalPersonProfile>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<LegalPersonProfile>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<LegalPersonProfile>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<LegalPersonProfile> actual, IQueryable<LegalPersonProfile> outdated)
        {
            const string legalPersonProfileName = "NuClear.ValidationRules.Storage.Model.Facts.LegalPersonProfile";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from legalPersonProfile in actual.Union(outdated)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .InnerJoin(x => x.LegalPersonId == legalPersonProfile.LegalPersonId)
                select new RelationRecord(legalPersonProfileName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}

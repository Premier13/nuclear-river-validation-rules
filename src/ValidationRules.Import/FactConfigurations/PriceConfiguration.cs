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
    public sealed class PriceConfiguration : IEntityConfiguration, IRelationProvider<Price>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<Price>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<Price>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<Price> actual, IQueryable<Price> outdated)
        {
            const string priceName = "NuClear.ValidationRules.Storage.Model.Facts.Price";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from price in actual.Union(outdated)
                from order in dataConnection.GetTable<Order>()
                    .InnerJoin(x => x.AgileDistributionStartDate >= price.BeginDate && x.ProjectId == price.ProjectId)
                select new RelationRecord(priceName, orderName, order.Id);

            return orderRelations.ToList();
        }
    }
}

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
    public sealed class PricePositionConfiguration : IEntityConfiguration, IRelationProvider<PricePosition>
    {
        public void Apply(Cache cache)
            => cache.Entity<PricePosition>()
                .HasKey(x => x.Id);

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<PricePosition>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(Writer writer)
            => writer.Entity<PricePosition>()
                .HasRelationsProvider(this)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<PricePosition> actual, IQueryable<PricePosition> outdated)
        {
            const string pricePositionName = "NuClear.ValidationRules.Storage.Model.Facts.PricePosition";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from pricePosition in actual.Union(outdated)
                from orderPosition in dataConnection.GetTable<OrderPosition>()
                    .InnerJoin(x => x.PricePositionId == pricePosition.Id)
                select new RelationRecord(pricePositionName, orderName, orderPosition.OrderId);

            return orderRelations.ToList();
        }
    }
}

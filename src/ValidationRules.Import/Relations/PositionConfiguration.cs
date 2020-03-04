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
    public sealed class PositionConfiguration : IEntityConfiguration, IRelationProvider<Position>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<Position>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<Position>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<Position> actual, IQueryable<Position> outdated)
        {
            const string positionName = "NuClear.ValidationRules.Storage.Model.Facts.Position";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelationsByOpa =
                from position in actual.Union(outdated)
                from opa in dataConnection.GetTable<OrderPositionAdvertisement>()
                    .InnerJoin(x => x.PositionId == position.Id)
                select new RelationRecord(positionName, orderName, opa.OrderId);

            var orderRelationsByPricePosition =
                from position in actual.Union(outdated)
                from pricePosition in dataConnection.GetTable<PricePosition>()
                    .InnerJoin(x => x.PositionId == position.Id)
                from orderPosition in dataConnection.GetTable<OrderPosition>()
                    .InnerJoin(x => x.PricePositionId == pricePosition.Id)
                select new RelationRecord(positionName, orderName, orderPosition.OrderId);

            var result = new List<RelationRecord>();
            result.AddRange(orderRelationsByOpa);
            result.AddRange(orderRelationsByPricePosition);
            return result;
        }
    }
}

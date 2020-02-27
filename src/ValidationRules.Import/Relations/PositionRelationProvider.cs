using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class PositionRelationProvider : IRelationProvider<Position>
    {
        private const string PositionName = "NuClear.ValidationRules.Storage.Model.Facts.Position";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<Position> entities)
        {
            var orderRelationsByOpa =
                from position in entities
                from opa in dataConnection.GetTable<OrderPositionAdvertisement>()
                    .Where(x => x.PositionId == position.Id)
                select new RelationRecord(PositionName, OrderName, opa.OrderId);

            var orderRelationsByPricePosition =
                from position in entities
                from pricePosition in dataConnection.GetTable<PricePosition>()
                    .Where(x => x.PositionId == position.Id)
                from orderPosition in dataConnection.GetTable<OrderPosition>()
                    .Where(x => x.PricePositionId == pricePosition.Id)
                select new RelationRecord(PositionName, OrderName, orderPosition.OrderId);

            // todo: сравнить запросы (UnionAll ... Distinct) и (Union ... Distinct), выбрать подходящее решение.
            return orderRelationsByOpa.UnionAll(orderRelationsByPricePosition);
        }
    }
}
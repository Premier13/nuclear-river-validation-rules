using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class PositionChildRelationProvider : IRelationProvider<PositionChild>
    {
        private const string PositionName = "NuClear.ValidationRules.Storage.Model.Facts.Position";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<PositionChild> entities)
        {
            var orderRelations =
                from position in entities
                from pricePosition in dataConnection.GetTable<PricePosition>()
                    .Where(x => x.PositionId == position.MasterPositionId)
                from orderPosition in dataConnection.GetTable<OrderPosition>()
                    .Where(x => x.PricePositionId == pricePosition.Id)
                select new RelationRecord(PositionName, OrderName, orderPosition.OrderId);

            return orderRelations;
        }
    }
}
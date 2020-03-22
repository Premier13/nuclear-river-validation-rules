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
    public sealed class PositionChildConfiguration : IEntityConfiguration, IRelationProvider<PositionChild>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<PositionChild>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.ChildPositionId, x.MasterPositionId});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<PositionChild>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasGroupKey(x => new {x.MasterPositionId});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<PositionChild> actual, IQueryable<PositionChild> outdated)
        {
            const string positionName = "NuClear.ValidationRules.Storage.Model.Facts.Position";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

            var orderRelations =
                from position in actual.Union(outdated)
                from pricePosition in dataConnection.GetTable<PricePosition>()
                    .InnerJoin(x => x.PositionId == position.MasterPositionId)
                from orderPosition in dataConnection.GetTable<OrderPosition>()
                    .InnerJoin(x => x.PricePositionId == pricePosition.Id)
                select new RelationRecord(positionName, orderName, orderPosition.OrderId);

            return orderRelations.ToList();
        }
    }
}

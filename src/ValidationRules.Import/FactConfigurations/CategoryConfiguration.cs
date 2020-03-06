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
    public sealed class CategoryConfiguration : IEntityConfiguration, IRelationProvider<Category>
    {
        public void Apply(FluentMappingBuilder builder)
        {
            builder.Entity<Category>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);
        }

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
        {
            cacheSaver.Add(typeof(Category), new CategoryWriter(enableRelations ? this : null));
        }

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<Category> actual, IQueryable<Category> outdated)
        {
            const string categoryName = "NuClear.ValidationRules.Storage.Model.Facts.Category";
            const string orderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";
            const string firmName = "NuClear.ValidationRules.Storage.Model.Facts.Firm";

            var orderRelations =
                from category in actual.Union(outdated)
                from opa in dataConnection.GetTable<OrderPositionAdvertisement>()
                    .InnerJoin(x => x.CategoryId == category.Id)
                select new RelationRecord(categoryName, orderName, opa.OrderId);

            var firmRelations =
                from category in actual.Union(outdated)
                from opa in dataConnection.GetTable<OrderPositionAdvertisement>()
                    .InnerJoin(x => x.CategoryId == category.Id)
                from order in dataConnection.GetTable<Order>()
                    .InnerJoin(x => x.Id == opa.OrderId)
                select new RelationRecord(categoryName, firmName, order.FirmId);

            var result = new List<RelationRecord>();
            result.AddRange(orderRelations);
            result.AddRange(firmRelations);
            // в оригинале были ещё тематикти, но мы их собрались удалять.
            return result;
        }
    }
}

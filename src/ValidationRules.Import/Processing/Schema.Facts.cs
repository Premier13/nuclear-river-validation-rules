using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.Facts;

namespace NuClear.ValidationRules.Import.Processing
{
    public static partial class Schema
    {
        private static FluentMappingBuilder RegisterFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<OrderConsistency>()
                .HasSchemaName(FactsSchema);

            builder.Entity<Order>()
                .HasSchemaName(FactsSchema);

            builder.Entity<OrderPosition>()
                .HasSchemaName(FactsSchema);

            builder.Entity<OrderPositionAdvertisement>()
                .HasSchemaName(FactsSchema);

            builder.Entity<QuantitativeRule>()
                .HasSchemaName(FactsSchema);

            return builder;
        }
    }
}

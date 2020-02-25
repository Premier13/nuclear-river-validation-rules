using System.Xml.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

namespace NuClear.ValidationRules.Import.Model
{
    public static partial class Schema
    {
        private static MappingSchema RegisterDataTypes(this MappingSchema schema)
        {
            schema.SetDataType(typeof(decimal), new SqlDataType(DataType.Decimal, 19, 4));
            schema.SetDataType(typeof(decimal?), new SqlDataType(DataType.Decimal, 19, 4));
            schema.SetDataType(typeof(string), new SqlDataType(DataType.NVarChar, int.MaxValue));
            schema.SetDataType(typeof(byte[]), new SqlDataType(DataType.VarBinary, int.MaxValue));

            // XDocument mapping to nvarchar
            schema.SetDataType(typeof(XDocument), new SqlDataType(DataType.NVarChar, 4000));
            schema.SetConvertExpression<string, XDocument>(x => XDocument.Parse(x));
            schema.SetConvertExpression<XDocument, string>(x => x.ToString(SaveOptions.DisableFormatting));
            schema.SetConvertExpression<XDocument, DataParameter>(x => new DataParameter
                {DataType = DataType.NVarChar, Value = x.ToString(SaveOptions.DisableFormatting)});

            return schema;
        }

        private static FluentMappingBuilder RegisterFinancialData(this FluentMappingBuilder builder)
        {
            builder.Entity<FinancialData.Account>()
                .HasSchemaName(FactsFinancialDataSchema)
                .HasPrimaryKey(x => x.Id);

            builder.Entity<FinancialData.AccountDetail>()
                .HasSchemaName(FactsFinancialDataSchema)
                .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<Facts.OrderConsistency>()
                .HasSchemaName(FactsSchema)
                .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterService(this FluentMappingBuilder builder)
        {
            builder.Entity<Service.ConsumerState>()
                .HasSchemaName(ServiceSchema)
                .HasPrimaryKey(x => new {x.Topic, x.Partition})
                .Property(x => x.Topic).HasLength(128).IsNullable(false);

            builder.Entity<Events.EventRecord>()
                .HasSchemaName(ServiceSchema)
                .HasPrimaryKey(x => x.Id)
                .HasIdentity(x => x.Id);

            return builder;
        }
    }
}

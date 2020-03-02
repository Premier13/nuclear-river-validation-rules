using System.Xml.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

namespace NuClear.ValidationRules.Import.Processing
{
    // fixme: копипаста, потому что разные версии linq2db
    public partial class Schema
    {
        private const string FactsSchema = "Facts";
        public const string ServiceSchema = "Service";
        public const string PersistentFactsSchema = "PersistentFacts";

        public static FluentMappingBuilder Common { get; } =
            new MappingSchema(nameof(Common), new SqlServerMappingSchema())
                .RegisterDataTypes()
                .GetFluentMappingBuilder()
                .RegisterFacts()
                .RegisterService();

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

        private static FluentMappingBuilder RegisterService(this FluentMappingBuilder builder)
        {
            builder.Entity<Model.Service.ConsumerState>()
                .HasSchemaName(ServiceSchema)
                .HasPrimaryKey(x => new {x.Topic, x.Partition})
                .Property(x => x.Topic).HasLength(128).IsNullable(false);

            builder.Entity<Model.Service.EventRecord>()
                .HasSchemaName(ServiceSchema)
                .HasPrimaryKey(x => x.Id)
                .HasIdentity(x => x.Id);

            return builder;
        }
    }
}

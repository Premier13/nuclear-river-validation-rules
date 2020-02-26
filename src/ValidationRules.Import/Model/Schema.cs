using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

namespace NuClear.ValidationRules.Import.Model
{
    // fixme: копипаста, потому что разные версии linq2db
    public partial class Schema
    {
        private const string FactsSchema = "Facts";
        private const string ServiceSchema = "Service";
        private const string PersistentFactsSchema = "PersistentFacts";

        public static MappingSchema Common { get; } =
            new MappingSchema(nameof(Facts), new SqlServerMappingSchema())
                .RegisterDataTypes()
                .GetFluentMappingBuilder()
                .RegisterPersistentFacts()
                .RegisterFacts()
                .RegisterService()
                .MappingSchema;

    }
}

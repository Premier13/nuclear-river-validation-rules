using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.Processing
{
    public class DataConnectionFactory
    {
        private readonly string _connectionString;
        private readonly MappingSchema _schema;

        public DataConnectionFactory(string connectionString, IReadOnlyCollection<IEntityConfiguration> configurations)
        {
            _connectionString = connectionString;
            _schema = configurations.Aggregate(Schema.Common, (builder, provider) =>
            {
                provider.Apply(builder);
                return builder;
            }).MappingSchema;
        }

        public DataConnection Create()
        {
            var connection = new DataConnection(
                SqlServerTools.GetDataProvider(SqlServerVersion.v2012),
                _connectionString);
            connection.AddMappingSchema(_schema);

            // DataConnection.TurnTraceSwitchOn();
            // DataConnection.WriteTraceLine = (s1, s2) =>
            // {
            //     lock (this)
            //     {
            //         File.AppendAllLines(@"c:\dev\log.txt", new[]
            //         {
            //             "===================================",
            //             s1
            //         });
            //     }
            // };

            return connection;
        }
    }
}

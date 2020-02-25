using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

namespace NuClear.ValidationRules.Import.SqlStore
{
    public class DataConnectionFactory
    {
        private readonly string _connectionString;
        private readonly MappingSchema _schema;

        public DataConnectionFactory(string connectionString, MappingSchema schema)
        {
            _connectionString = connectionString;
            _schema = schema;
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
            //     File.AppendAllLines(@"c:\dev\log.txt", new[]
            //     {
            //         "===================================",
            //         x.Elapsed.TotalSeconds.ToString("F2"),
            //         s1
            //     });
            // };

            return connection;
        }
    }
}

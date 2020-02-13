using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using NuClear.Replication.Core.Tenancy;
using NuClear.ValidationRules.Hosting.Common.Identities.Connections;
using NuClear.ValidationRules.SingleCheck.Tenancy;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.Querying.Host.Tenancy
{
    public class DataConnectionProvider : IDataConnectionProvider
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly ITenantConnectionStringSettings _connectionStringSettings;

        public DataConnectionProvider(ITenantProvider tenantProvider,
            ITenantConnectionStringSettings connectionStringSettings)
        {
            _tenantProvider = tenantProvider;
            _connectionStringSettings = connectionStringSettings;
        }

        public DataConnection CreateErmConnection()
        {
            var dataConnection = new DataConnection(
                SqlServerTools.GetDataProvider(SqlServerVersion.v2012),
                _connectionStringSettings.GetConnectionString(ErmConnectionStringIdentity.Instance, _tenantProvider.Current));
            dataConnection.AddMappingSchema(Schema.Erm);
            return dataConnection;
        }

        public DataConnection CreateVrConnection()
        {
            var dataConnection = new DataConnection(
                SqlServerTools.GetDataProvider(SqlServerVersion.v2012),
                _connectionStringSettings.GetConnectionString(ValidationRulesConnectionStringIdentity.Instance, _tenantProvider.Current));
            // Schema.Facts needed for Facts.EntityName table
            dataConnection.AddMappingSchema(Schema.Facts);
            dataConnection.AddMappingSchema(Schema.Messages);
            return dataConnection;
        }
    }
}

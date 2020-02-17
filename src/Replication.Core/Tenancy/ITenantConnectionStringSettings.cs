using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Core.Tenancy
{
    public interface ITenantConnectionStringSettings
    {
        string GetConnectionString(IConnectionStringIdentity identity, Tenant tenant);
        string GetConnectionString(IConnectionStringIdentity identity);
    }
}

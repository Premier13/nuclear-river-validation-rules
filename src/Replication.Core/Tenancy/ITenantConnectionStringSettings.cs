using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Core.Tenancy
{
    public interface ITenantConnectionStringSettings : ISettings
    {
        string GetConnectionString(IConnectionStringIdentity identity, Tenant tenant);
        string GetConnectionString(IConnectionStringIdentity identity);
    }
}

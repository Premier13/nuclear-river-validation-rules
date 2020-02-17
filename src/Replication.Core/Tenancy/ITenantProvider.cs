namespace NuClear.Replication.Core.Tenancy
{
    public interface ITenantProvider
    {
        Tenant Current { get; }
    }
}
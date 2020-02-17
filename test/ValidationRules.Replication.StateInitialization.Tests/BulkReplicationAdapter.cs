using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Replication.Core.Tenancy;
using NuClear.StateInitialization.Core.Actors;
using NuClear.ValidationRules.Hosting.Common.Settings;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private readonly T _key;
        private readonly ITenantConnectionStringSettings _connectionStringSettings;

        public BulkReplicationAdapter()
        {
            _key = new T();
            _connectionStringSettings = new TenantConnectionStringSettings();
        }

        public void Act()
        {
            var bulkReplicationActor = new BulkReplicationActor(_connectionStringSettings);
            bulkReplicationActor.ExecuteCommands(new[] { _key.Command });
        }
    }
}

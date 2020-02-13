using System;
using System.Collections.Generic;
using System.Configuration;
using NuClear.Replication.Core.Tenancy;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Hosting.Common.Identities.Connections;

namespace NuClear.ValidationRules.Hosting.Common.Settings
{
    public class ConnectionStringSettings : IConnectionStringSettings, ISettingsAspect
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly ValidationRulesConnectionStringProvider _connectionStringProvider;

        public ConnectionStringSettings(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
            _connectionStringProvider = new ValidationRulesConnectionStringProvider();
        }

        public IReadOnlyDictionary<IConnectionStringIdentity, string> AllConnectionStrings
            => throw new NotSupportedException("Obsolete api method");

        public string GetConnectionString(IConnectionStringIdentity identity)
            => _connectionStringProvider.GetConnectionString(identity, _tenantProvider.Current);
    }

    public sealed class TenantConnectionStringSettings : ITenantConnectionStringSettings, IConnectionStringSettings, ISettingsAspect
    {
        private readonly ValidationRulesConnectionStringProvider _connectionStringProvider =
            new ValidationRulesConnectionStringProvider();

        public IReadOnlyDictionary<IConnectionStringIdentity, string> AllConnectionStrings
            => throw new NotSupportedException("Obsolete api method");

        public string GetConnectionString(IConnectionStringIdentity identity, Tenant tenant)
            => _connectionStringProvider.GetConnectionString(identity, tenant);

        public string GetConnectionString(IConnectionStringIdentity identity)
            => _connectionStringProvider.GetConnectionString(identity);
    }

    internal sealed class ValidationRulesConnectionStringProvider
    {
        public string GetConnectionString(IConnectionStringIdentity identity, Tenant tenant)
        {
            switch (identity)
            {
                case ErmConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings[$"Erm.{tenant:G}"].ConnectionString;

                case ServiceBusConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings[$"ServiceBus.{tenant:G}"].ConnectionString;

                default:
                    return GetConnectionString(identity);
            }
        }

        public string GetConnectionString(IConnectionStringIdentity identity)
        {
            switch (identity)
            {
                case ErmConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings["Erm"].ConnectionString;

                case ValidationRulesConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings["ValidationRules"].ConnectionString;

                case KafkaConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings["Kafka"].ConnectionString;

                case LoggingConnectionStringIdentity _:
                    return ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;

                default:
                    throw new ArgumentException($"Unsupported connection string identity {identity.GetType().Name}", nameof(identity));
            }
        }
    }
}

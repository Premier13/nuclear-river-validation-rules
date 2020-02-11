using System.Collections.Specialized;
using System.Configuration;

using Jobs.RemoteControl.Settings;

using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.Core.Settings;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.Telemetry.Logstash;
using NuClear.ValidationRules.Hosting.Common.Settings;
using Quartz.Impl;

namespace NuClear.ValidationRules.Replication.Host.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase, IReplicationSettings, ISqlStoreSettingsAspect
    {
        private readonly IntSetting _replicationBatchSize = ConfigFileSetting.Int.Required("ReplicationBatchSize");
        private readonly IntSetting _sqlCommandTimeout = ConfigFileSetting.Int.Required("SqlCommandTimeout");

        public ReplicationServiceSettings()
        {
            var quartzProperties = (NameValueCollection)ConfigurationManager.GetSection(StdSchedulerFactory.ConfigurationSectionName);

            Aspects.Use<TenantConnectionStringSettings>()
                   .Use<ServiceBusMessageLockRenewalSettings>()
                   .Use<EnvironmentSettingsAspect>()
                   .Use<QuartzSettingsAspect>()
                   .Use<ArchiveVersionsSettings>()
                   .Use<LogstashSettingsAspect>()
                   .Use(new TaskServiceRemoteControlSettings(quartzProperties));
        }

        public int ReplicationBatchSize => _replicationBatchSize.Value;

        public int SqlCommandTimeout => _sqlCommandTimeout.Value;
    }
}

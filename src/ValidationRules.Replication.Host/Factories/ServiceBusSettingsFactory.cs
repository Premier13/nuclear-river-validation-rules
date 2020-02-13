using System;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.OperationsProcessing.Facts.ErmFactsFlow;

namespace NuClear.ValidationRules.Replication.Host.Factories
{
    public sealed class ServiceBusSettingsFactory : IServiceBusSettingsFactory
    {
        private static readonly StringSetting _ermOperationsFlowTopic = ConfigFileSetting.String.Optional("ErmEventsFlowTopic", "topic.performedoperations");

        private readonly IConnectionStringSettings _connectionStringSettings;

        public ServiceBusSettingsFactory(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public IServiceBusMessageReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            if (ErmFactsFlow.Instance.Equals(messageFlow))
                return new Settings
                {
                    ConnectionString = ServiceBusConnectionString,
                    TransportEntityPath = _ermOperationsFlowTopic.Value,
                };

            throw new ArgumentException($"Flow '{messageFlow.Description}' settings for MS ServiceBus are undefined");
        }

        public IServiceBusMessageSenderSettings CreateSenderSettings(IMessageFlow messageFlow)
        {
            throw new ArgumentException($"Flow '{messageFlow.Description}' settings for MS ServiceBus are undefined");
        }

        private string ServiceBusConnectionString
            => _connectionStringSettings.GetConnectionString(ServiceBusConnectionStringIdentity.Instance);

        private class Settings : IServiceBusMessageReceiverSettings
        {
            public string TransportEntityPath { get; set; }
            public string ConnectionString { get; set; }
            public int ConnectionsCount { get; } = 1;
            public bool UseTransactions { get; } = true;
        }
    }
}

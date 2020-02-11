using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core.Tenancy;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.ValidationRules.Hosting.Common;
using NuClear.ValidationRules.Hosting.Common.Identities.Connections;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Tenancy;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.SingleCheck.Tenancy;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using IConnectionStringSettings = NuClear.Storage.API.ConnectionStrings.IConnectionStringSettings;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            var settings = new QueryingServiceSettings();

            var environmentSettings = settings.AsSettings<IEnvironmentSettings>();
            var connectionStringSettings = settings.AsSettings<IConnectionStringSettings>();

            return new UnityContainer()
                .ConfigureSettingsAspects(settings)
                .ConfigureTracing()
                .ConfigureDataAccess()
                .RegisterImplementersCollection<IMessageComposer>()
                .RegisterImplementersCollection<IDistinctor>()
                .ConfigureSeverityProvider()
                .ConfigureNameResolvingService()
                .ConfigureSingleCheck()
                .ConfigureOperationsProcessing(environmentSettings, connectionStringSettings);
        }

        private static IUnityContainer ConfigureDataAccess(this IUnityContainer container)
        {
            return container
                .RegisterType<ITenantProvider, HttpContextTenantProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IDataConnectionProvider, DataConnectionProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<ValidationResultRepositiory>(new PerResolveLifetimeManager());
        }

        private static IUnityContainer RegisterImplementersCollection<TContract>(this IUnityContainer container)
            where TContract : class
        {
            var interfaceType = typeof(TContract);
            var implementerTypes = interfaceType.Assembly.GetTypes()
                .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

            container.RegisterType(typeof(IReadOnlyCollection<TContract>),
                new InjectionFactory(c => implementerTypes.Select(t => c.Resolve(t)).Cast<TContract>().ToList()));

            return container;
        }

        private static IUnityContainer ConfigureSeverityProvider(this IUnityContainer container) =>
            container.RegisterType<IMessageSeverityProvider, MessageSeverityProvider>(
                new ContainerControlledLifetimeManager());

        private static IUnityContainer ConfigureNameResolvingService(this IUnityContainer container)
        {
            var interfaceType = typeof(IEntityType);
            var type = typeof(EntityTypeOrder);
            var entityTypes = type.Assembly.GetTypes()
                .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

            container.RegisterType(typeof(IReadOnlyCollection<IEntityType>),
                new InjectionFactory(c => entityTypes.Select(t => c.Resolve(t)).Cast<IEntityType>().ToList()));

            return container;
        }

        private static IUnityContainer ConfigureSingleCheck(this IUnityContainer container)
        {
            container.RegisterType<PipelineFactory>();
            container.RegisterType<VersionHelper>(new ContainerControlledLifetimeManager());

            return container;
        }

        private static IUnityContainer ConfigureOperationsProcessing(this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings)
        {
            var kafkaSettingsFactory =
                new KafkaSettingsFactory(new Dictionary<IMessageFlow, string>
                    {
                        [AliasForAmsFactsFlow.Instance] =
                            connectionStringSettings.GetConnectionString(AmsConnectionStringIdentity.Instance)
                    },
                    environmentSettings);

            return container.RegisterInstance<IKafkaSettingsFactory>(kafkaSettingsFactory)
                .RegisterType<KafkaMessageFlowInfoProvider>(new ContainerControlledLifetimeManager());
        }
    }
}

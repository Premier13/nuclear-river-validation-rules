using System;
using Microsoft.Practices.Unity;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureTracing(this IUnityContainer container)
        {
            return container.RegisterType<ITracer>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(CreateTracer));
        }

        private static ITracer CreateTracer(IUnityContainer container)
        {
            var environmentSettings = container.Resolve<IEnvironmentSettings>();
            var connectionStringSettings = container.Resolve<IConnectionStringSettings>();
            var logstashConnectionString = new Uri(
                connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance));

            return Log4NetTracerBuilder.Use
                .ApplicationXmlConfig
                .Console
                .EventLog
                .WithGlobalProperties(x =>
                    x.Property("Environment", environmentSettings.EnvironmentName)
                        .Property(TracerContextKeys.EntryPoint, environmentSettings.EntryPointName)
                        .Property(TracerContextKeys.EntryPointHost, NetworkInfo.ComputerFQDN)
                        .Property(TracerContextKeys.EntryPointInstanceId, Guid.NewGuid().ToString()))
                .Logstash(logstashConnectionString, appender => { appender.LogstashLayout.IncrementalCounter = true; })
                .Build;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Replication.Host.DI;
using NuClear.CustomerIntelligence.Replication.Host.Settings;
using NuClear.Jobs.Schedulers;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.River.Hosting.Interactive;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net;
using NuClear.Tracing.Log4Net.Config;

namespace NuClear.CustomerIntelligence.Replication.Host
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var isDebuggerMode = IsDebuggerMode(args);
            if (isDebuggerMode && !Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            var settingsContainer = new ReplicationServiceSettings();
            var environmentSettings = settingsContainer.AsSettings<IEnvironmentSettings>();
            var squirrelSettings = settingsContainer.AsSettings<ISquirrelSettings>();
            var connectionStringSettings = settingsContainer.AsSettings<IConnectionStringSettings>();

            var tracerContextEntryProviders =
                    new ITracerContextEntryProvider[]
                    {
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPoint, environmentSettings.HostName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new TracerContextSelfHostedEntryProvider(TracerContextKeys.Required.UserAccount)
                    };

            var tracerContextManager = new TracerContextManager(tracerContextEntryProviders);
            var tracer = Log4NetTracerBuilder.Use
                                             .DefaultXmlConfig
                                             .Console
                                             .EventLog
                                             .Logstash(new Uri(connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance)))
                                             .Build;

            IUnityContainer container = null;
            try
            {
                container = Bootstrapper.ConfigureUnity(settingsContainer, tracer, tracerContextManager);
                var scheduleManager = container.Resolve<ISchedulerManager>();

                var hostParameters = new HostParameters(environmentSettings.HostName, environmentSettings.HostDisplayName, squirrelSettings.UpdateServerUrl);
                var host = new River.Hosting.Interactive.Host(hostParameters, scheduleManager);
                host.ConfigureAndRun();
            }
            finally
            {
                container?.Dispose();
            }
        }

        private static bool IsDebuggerMode(IEnumerable<string> args)
        {
            return args.Any(x => x.LastIndexOf("debug", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}

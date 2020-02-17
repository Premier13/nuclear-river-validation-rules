using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

using Microsoft.Practices.Unity;

using NuClear.Jobs.Schedulers;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.Replication.Host.DI;
using NuClear.ValidationRules.Replication.Host.Settings;

namespace NuClear.ValidationRules.Replication.Host
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

            IUnityContainer container = null;
            try
            {
                container = Bootstrapper.ConfigureUnity();
                var schedulerManagers = container.ResolveAll<ISchedulerManager>().ToList();
                if (IsConsoleMode(args))
                {
                    foreach (var schedulerManager in schedulerManagers)
                    {
                        schedulerManager.Start();
                    }

                    Console.WriteLine("ERM Validation Rules Replication service successfully started.");
                    Console.WriteLine("Press ENTER to stop...");

                    Console.ReadLine();

                    Console.WriteLine("ERM Validation Rules Replication service is stopping...");

                    foreach (var schedulerManager in schedulerManagers)
                    {
                        schedulerManager.Stop();
                    }

                    Console.WriteLine("ERM Validation Rules Replication service stopped successfully. Press ENTER to exit...");
                    Console.ReadLine();
                }
                else
                {
                    using (var replicationService = new ReplicationService(schedulerManagers))
                    {
                        ServiceBase.Run(replicationService);
                    }
                }
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

        private static bool IsConsoleMode(IEnumerable<string> args)
        {
            return args.Any(x => x.LastIndexOf("console", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}

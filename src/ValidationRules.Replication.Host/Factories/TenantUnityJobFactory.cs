using System;
using System.Collections.Concurrent;
using Microsoft.Practices.Unity;
using NuClear.Replication.Core.Tenancy;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Hosting.Common.Settings;
using Quartz;
using Quartz.Spi;

namespace NuClear.ValidationRules.Replication.Host.Factories
{
    public class TenantUnityJobFactory : IJobFactory
    {
        private const string TenantJobParameterName = "Tenant";

        private readonly IUnityContainer _container;
        private readonly ConcurrentDictionary<IJob, IUnityContainer> _containerMap;

        public TenantUnityJobFactory(IUnityContainer container)
        {
            _container = container;
            _containerMap = new ConcurrentDictionary<IJob, IUnityContainer>();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var childContainer = _container.CreateChildContainer();

            var tenant = GetTenant(bundle);
            if (tenant != null)
            {
                childContainer.RegisterInstance<Tenant>((Tenant) Enum.Parse(typeof(Tenant), tenant, true));
                childContainer.RegisterType<ITenantProvider, ScopedTenantProvider>();
                childContainer.RegisterType<IConnectionStringSettings, ConnectionStringSettings>();
            }
            else
            {
                childContainer.RegisterType<ITenantProvider, DefaultTenantProvider>();
            }

            var key = (IJob) childContainer.Resolve(bundle.JobDetail.JobType);
            _containerMap.TryAdd(key, childContainer);
            return key;
        }

        public void ReturnJob(IJob job)
        {
            if (_containerMap.TryRemove(job, out var container))
                container.Dispose();
        }

        private static string GetTenant(TriggerFiredBundle bundle)
        {
            var triggerParameter = bundle.JobDetail.JobDataMap.GetString(TenantJobParameterName);
            if (!string.IsNullOrWhiteSpace(triggerParameter))
                return triggerParameter;

            var jobParameter = bundle.Trigger.JobDataMap.GetString(TenantJobParameterName);
            if (!string.IsNullOrWhiteSpace(jobParameter))
                return jobParameter;

            return null;
        }

        private sealed class ScopedTenantProvider : ITenantProvider
        {
            public ScopedTenantProvider(Tenant current)
            {
                Current = current;
            }

            public Tenant Current { get; }
        }

        private sealed class DefaultTenantProvider : ITenantProvider
        {
            public Tenant Current => throw new NotSupportedException("Tetant is not configured for this job.");
        }
    }
}

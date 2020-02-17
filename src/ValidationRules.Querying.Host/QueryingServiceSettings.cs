using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.ValidationRules.Hosting.Common.Settings;

namespace NuClear.ValidationRules.Querying.Host
{
    internal sealed class QueryingServiceSettings : SettingsContainerBase
    {
        public QueryingServiceSettings()
        {
            Aspects
                .Use<TenantConnectionStringSettings>()
                .Use<EnvironmentSettingsAspect>();
        }
    }
}

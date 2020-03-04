using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public class ProducerFactory
    {
        private static readonly int MaxCacheSize = 100000;
        private static readonly TimeSpan MaxCacheAge = TimeSpan.FromSeconds(10);

        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly IReadOnlyCollection<IEntityConfiguration> _configurations;
        private readonly bool _enableRelations;

        public ProducerFactory(DataConnectionFactory dataConnectionFactory, IReadOnlyCollection<IEntityConfiguration> configurations, bool enableRelations)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _configurations = configurations;
            _enableRelations = enableRelations;
        }

        public Producer Create()
        {
            var cache = new Cache(MaxCacheSize);
            var saver = new CacheSaver(_dataConnectionFactory);

            foreach (var configuration in _configurations)
                configuration.Apply(saver, _enableRelations);

            return new Producer(cache, saver, MaxCacheAge);
        }
    }
}

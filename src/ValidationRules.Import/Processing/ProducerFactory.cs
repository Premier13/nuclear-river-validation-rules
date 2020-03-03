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
        private readonly bool _ignoreRelations;

        public ProducerFactory(DataConnectionFactory dataConnectionFactory, IReadOnlyCollection<IEntityConfiguration> configurations, bool ignoreRelations)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _configurations = configurations;
            _ignoreRelations = ignoreRelations;
        }

        public Producer Create()
        {
            var cache = new Cache(MaxCacheSize, MaxCacheAge);
            var dataWriter = new Writer(_dataConnectionFactory, _ignoreRelations);

            foreach (var configuration in _configurations)
            {
                configuration.Apply(cache);
                configuration.Apply(dataWriter);
            }

            return new Producer(cache, dataWriter);
        }
    }
}

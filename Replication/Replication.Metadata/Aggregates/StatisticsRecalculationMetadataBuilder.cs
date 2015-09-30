﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Metadata.Aggregates
{
    public class StatisticsRecalculationMetadataBuilder<T> : MetadataElementBuilder<StatisticsRecalculationMetadataBuilder<T>, StatisticsRecalculationMetadata<T>>
    {
        private MapToObjectsSpecProvider<T, T> _mapSpecificationProviderForSource;
        private MapToObjectsSpecProvider<T, T> _mapSpecificationProviderForTarget;
        private Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> _findSpecificationProvider;
        private IEqualityComparer<T> _equalityComparer;

        protected override StatisticsRecalculationMetadata<T> Create()
        {
            return new StatisticsRecalculationMetadata<T>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                _findSpecificationProvider,
                _equalityComparer,
                Features);
        }

        public StatisticsRecalculationMetadataBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification)
        {
            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => sourceMappingSpecification.Map(q).Where(specification));
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<T> HasTarget(MapSpecification<IQuery, IQueryable<T>> targetMappingSpecification)
        {
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification)); ;
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<T> HasFilter(Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<T> HasFieldComparer(IEqualityComparer<T> equalityComparer)
        {
            _equalityComparer = equalityComparer;
            return this;
        }
    }
}
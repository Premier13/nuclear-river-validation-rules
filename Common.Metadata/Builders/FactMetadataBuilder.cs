﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Context;
using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Model.Common.Entities;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Builders
{
    public class FactMetadataBuilder<T> : MetadataElementBuilder<FactMetadataBuilder<T>, FactMetadata<T>>
        where T : class, IIdentifiable
    {
        private MapSpecification<IQuery, IQueryable<T>> _sourceMappingSpecification;

        protected override FactMetadata<T> Create()
        {
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource =
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _sourceMappingSpecification.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget = 
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new FactMetadata<T>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, Specs.Find.ByIds<T>, Features);
        }

        public FactMetadataBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            return this;
        }

        public FactMetadataBuilder<T> HasDependentAggregate<TAggregate>(
            Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            where TAggregate : EntityTypeBase<TAggregate>, new()
        {
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => dependentAggregateSpecProvider
                                              .Invoke(specification)
                                              .Map(q)
                                              .Select(id => PredicateFactory.EntityById(EntityTypeBase<TAggregate>.Instance, id))
                                              .Select(predicate => new RecalculateAggregate(predicate)));
            AddFeatures(new IndirectlyDependentAggregateFeature<T>(mapSpecificationProvider));
            return this;
        }

        public FactMetadataBuilder<T> HasMatchedAggregate<TAggregate>()
            where TAggregate : EntityTypeBase<TAggregate>, new()
        {
            // FIXME {all, 04.09.2015}: Слабое место - внутри спецификации идентификаторы, затем идём в базу за идентификаторами. Если достать их из спецификации в бузу хдить не потребуется.
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnCreate =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => PredicateFactory.EntityById(EntityTypeBase<TAggregate>.Instance, id))
                                           .Select(predicate => new InitializeAggregate(predicate)));

            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnUpdate =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => PredicateFactory.EntityById(EntityTypeBase<TAggregate>.Instance, id))
                                           .Select(predicate => new RecalculateAggregate(predicate)));

            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnDelete =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => PredicateFactory.EntityById(EntityTypeBase<TAggregate>.Instance, id))
                                           .Select(predicate => new DestroyAggregate(predicate)));

            AddFeatures(new DirectlyDependentAggregateFeature<T>(mapSpecificationProviderOnCreate, mapSpecificationProviderOnUpdate, mapSpecificationProviderOnDelete));
            return this;
        }
    }
}
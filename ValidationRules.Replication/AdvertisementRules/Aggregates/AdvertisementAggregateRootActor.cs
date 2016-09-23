﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class AdvertisementAggregateRootActor : EntityActorBase<Advertisement>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Advertisement.RequiredElementMissing> _requiredElementMissingBulkRepository;
        private readonly IBulkRepository<Advertisement.ElementInvalid> _elementInvalidBulkRepository;
        private readonly IBulkRepository<Advertisement.ElementDraft> _elementDraftBulkRepository;

        public AdvertisementAggregateRootActor(
            IQuery query,
            IBulkRepository<Advertisement> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Advertisement.RequiredElementMissing> requiredElementMissingBulkRepository,
            IBulkRepository<Advertisement.ElementInvalid> elementInvalidBulkRepository,
            IBulkRepository<Advertisement.ElementDraft> elementDraftBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new AdvertisementAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _requiredElementMissingBulkRepository = requiredElementMissingBulkRepository;
            _elementInvalidBulkRepository = elementInvalidBulkRepository;
            _elementDraftBulkRepository = elementDraftBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Advertisement.RequiredElementMissing>(_query, _requiredElementMissingBulkRepository, _equalityComparerFactory, new RequiredElementMissingAccessor(_query)),
                    new ValueObjectActor<Advertisement.ElementInvalid>(_query, _elementInvalidBulkRepository, _equalityComparerFactory, new ElementInvalidAccessor(_query)),
                    new ValueObjectActor<Advertisement.ElementDraft>(_query, _elementDraftBulkRepository, _equalityComparerFactory, new ElementDraftAccessor(_query)),
                };

        public sealed class AdvertisementAccessor : IStorageBasedDataObjectAccessor<Advertisement>
        {
            private readonly IQuery _query;

            public AdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Advertisement> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>()
                   select new Advertisement
                   {
                       Id = advertisement.Id,
                       Name = advertisement.Name,
                   };

            public FindSpecification<Advertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class RequiredElementMissingAccessor : IStorageBasedDataObjectAccessor<Advertisement.RequiredElementMissing>
        {
            private readonly IQuery _query;

            public RequiredElementMissingAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Advertisement.RequiredElementMissing> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>()
                   join element in _query.For<Facts::AdvertisementElement>() on advertisement.Id equals element.AdvertisementId
                   where element.IsEmpty // ЭРМ пустой
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>() on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   where elementTemplate.IsRequired // шаблон ЭРМ обязателен
                   select new Advertisement.RequiredElementMissing
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = elementTemplate.Id,
                   };

            public FindSpecification<Advertisement.RequiredElementMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement.RequiredElementMissing>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class ElementInvalidAccessor : IStorageBasedDataObjectAccessor<Advertisement.ElementInvalid>
        {
            private const int StatusInvalid = 2;

            private readonly IQuery _query;

            public ElementInvalidAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Advertisement.ElementInvalid> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>()
                   join template in _query.For<Facts::AdvertisementTemplate>() on advertisement.AdvertisementTemplateId equals template.Id
                   where advertisement.Id != template.DummyAdvertisementId // РМ - не заглушка
                   join element in _query.For<Facts::AdvertisementElement>() on advertisement.Id equals element.AdvertisementId
                   where element.Status == StatusInvalid // ЭРМ выверен с ошибками
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>() on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   where elementTemplate.NeedsValidation // ЭРМ должен быть выверен
                   select new Advertisement.ElementInvalid
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = elementTemplate.Id,
                   };

            public FindSpecification<Advertisement.ElementInvalid> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement.ElementInvalid>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class ElementDraftAccessor : IStorageBasedDataObjectAccessor<Advertisement.ElementDraft>
        {
            private const int StatusDraft = 3;

            private readonly IQuery _query;

            public ElementDraftAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Advertisement.ElementDraft> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>()
                   join template in _query.For<Facts::AdvertisementTemplate>() on advertisement.AdvertisementTemplateId equals template.Id
                   where advertisement.Id != template.DummyAdvertisementId // РМ - не заглушка
                   join element in _query.For<Facts::AdvertisementElement>() on advertisement.Id equals element.AdvertisementId
                   where element.Status == StatusDraft // ЭРМ - черновик
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>() on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   where elementTemplate.NeedsValidation // ЭРМ должен быть выверен
                   select new Advertisement.ElementDraft
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = elementTemplate.Id,
                   };

            public FindSpecification<Advertisement.ElementDraft> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement.ElementDraft>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }
    }
}

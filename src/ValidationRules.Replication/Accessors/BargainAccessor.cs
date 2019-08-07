﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class BargainAccessor : IStorageBasedDataObjectAccessor<Bargain>, IDataChangesHandler<Bargain>
    {
        private readonly IQuery _query;

        public BargainAccessor(IQuery query) => _query = query;

        public IQueryable<Bargain> GetSource() => _query
            .For<Erm::Bargain>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new Bargain
                {
                    Id = x.Id,
                    SignupDate = x.SignedOn,
                });

        public FindSpecification<Bargain> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<Bargain>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Bargain> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Bargain> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Bargain> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Bargain> dataObjects)
        {
            var bargainIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds = _query.For<OrderConsistency>()
                .Where(x => x.BargainId.HasValue && bargainIds.Contains(x.BargainId.Value))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(Bargain), typeof(Order), orderIds)};
        }
    }
}
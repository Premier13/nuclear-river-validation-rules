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
    public sealed class PricePositionAccessor : IStorageBasedDataObjectAccessor<PricePosition>, IDataChangesHandler<PricePosition>
    {
        private readonly IQuery _query;

        public PricePositionAccessor(IQuery query) => _query = query;

        public IQueryable<PricePosition> GetSource() => _query
            .For<Erm::PricePosition>()
            .Select(x => new PricePosition
            {
                Id = x.Id,
                PriceId = x.PriceId,
                PositionId = x.PositionId,
                IsActiveNotDeleted = x.IsActive && !x.IsDeleted
            });

        public FindSpecification<PricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<PricePosition>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<PricePosition> dataObjects)
        {
            var pricePositionIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds =
                (from op in _query.For<OrderPosition>().Where(x => pricePositionIds.Contains(x.PricePositionId))
                join order in _query.For<Order>() on op.OrderId equals order.Id
                select order.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(PricePosition), typeof(Order), orderIds)};
        }
    }
}
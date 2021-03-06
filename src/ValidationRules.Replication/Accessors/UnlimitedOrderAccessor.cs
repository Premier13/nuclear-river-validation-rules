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
    public sealed class UnlimitedOrderAccessor : IStorageBasedDataObjectAccessor<UnlimitedOrder>, IDataChangesHandler<UnlimitedOrder>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public UnlimitedOrderAccessor(IQuery query) => _query = query;

        public IQueryable<UnlimitedOrder> GetSource()
            => from x in _query.For<Erm::UnlimitedOrder>()
               where x.IsActive
               select new UnlimitedOrder
                   {
                       OrderId = x.OrderId,
                       PeriodStart = x.PeriodStart,
                       PeriodEnd = x.PeriodEnd + OneSecond,
                   };

        public FindSpecification<UnlimitedOrder> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<UnlimitedOrder>.Contains(x => x.OrderId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<UnlimitedOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<UnlimitedOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<UnlimitedOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<UnlimitedOrder> dataObjects)
        {
            var orderIds = dataObjects.Select(x => x.OrderId).ToHashSet();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(UnlimitedOrder), typeof(Order), orderIds)};
        }
    }
}
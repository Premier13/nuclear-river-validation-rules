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
    public sealed class OrderScanFileAccessor : IStorageBasedDataObjectAccessor<OrderScanFile>, IDataChangesHandler<OrderScanFile>
    {
        private readonly IQuery _query;

        public OrderScanFileAccessor(IQuery query) => _query = query;

        public IQueryable<OrderScanFile> GetSource() => _query
            .For<Erm::OrderFile>()
            .Where(x => x.IsActive && !x.IsDeleted && x.FileKind == Erm::OrderFile.OrderScan)
            .Select(x => new OrderScanFile
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                });

        public FindSpecification<OrderScanFile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<OrderScanFile>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderScanFile> dataObjects)
        {
            var orderIds = dataObjects.Select(x => x.OrderId);

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(OrderScanFile), typeof(Order), orderIds.ToHashSet())};
        }
    }
}
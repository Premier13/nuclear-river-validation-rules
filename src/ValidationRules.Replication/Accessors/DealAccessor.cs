using System;
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
    public sealed class DealAccessor : IStorageBasedDataObjectAccessor<Deal>, IDataChangesHandler<Deal>
    {
        private readonly IQuery _query;

        public DealAccessor(IQuery query) => _query = query;

        public IQueryable<Deal> GetSource() => _query
            .For<Erm::Deal>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new Deal
                {
                    Id = x.Id
                });

        public FindSpecification<Deal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<Deal>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Deal> dataObjects)
        {
            var dealIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds = _query.For<OrderConsistency>()
                .Where(x => dealIds.Contains(x.DealId.Value))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(Deal), typeof(Order), orderIds)};
        }
    }
}
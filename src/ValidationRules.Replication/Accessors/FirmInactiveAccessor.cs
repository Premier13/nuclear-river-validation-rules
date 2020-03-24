using System;
using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class FirmInactiveAccessor : IMemoryBasedDataObjectAccessor<FirmInactive>, IDataChangesHandler<FirmInactive>
    {
        private readonly IQuery _query;

        public FirmInactiveAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<FirmInactive> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .Cast<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<FirmDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            var result = dtos
                .Where(Specs.Find.InfoRussia.Firm.Inactive)
                .Select(x => new FirmInactive
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    IsClosedForAscertainment = x.ClosedForAscertainment
                }).ToList();

            return result;
        }

        public FindSpecification<FirmInactive> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmDto>()
                .Select(x => x.Id).ToHashSet();
            return new FindSpecification<FirmInactive>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmInactive> dataObjects)
        {
            var firmIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds = _query.For<Order>()
                .Where(x => firmIds.Contains(x.FirmId))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(FirmInactive), typeof(Order), orderIds)};
        }
    }
}
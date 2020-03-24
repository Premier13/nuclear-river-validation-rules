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
    public sealed class FirmAddressInactiveAccessor: IMemoryBasedDataObjectAccessor<FirmAddressInactive>, IDataChangesHandler<FirmAddressInactive>
    {
        private readonly IQuery _query;

        public FirmAddressInactiveAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<FirmAddressInactive> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .Cast<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<FirmAddressDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            var result = dtos
                .Where(Specs.Find.InfoRussia.FirmAddress.Inactive)
                .Select(x => new FirmAddressInactive
                {
                    Id = x.Id,
                    
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    IsClosedForAscertainment = x.ClosedForAscertainment
                }).ToList();

            return result;
        }

        public FindSpecification<FirmAddressInactive> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmAddressDto>()
                .Select(x => x.Id).ToHashSet();
            return new FindSpecification<FirmAddressInactive>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressInactive> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressInactive> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.Id).ToHashSet();
            
            var orderIds = _query.For<OrderPositionAdvertisement>()
                .Where(x => firmAddressIds.Contains(x.FirmAddressId.Value))
                .Select(x => x.OrderId)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(FirmAddress), typeof(Order), orderIds)};
        }
    }
}
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
    public sealed class FirmAddressAccessor: IMemoryBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<FirmAddress> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .Cast<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<FirmAddressDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            var result = dtos
                .Where(Specs.Find.InfoRussia.FirmAddress.Active)
                .Select(x => new FirmAddress
                {
                    Id = x.Id,
                    FirmId = x.FirmId,
                    IsLocatedOnTheMap = x.IsLocatedOnTheMap,
                    EntranceCode = x.EntranceCode,
                    BuildingId = x.BuildingId
                }).ToList();

            return result;
        }

        public FindSpecification<FirmAddress> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmAddressDto>()
                .Select(x => x.Id).ToHashSet();
            return new FindSpecification<FirmAddress>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var firmIds = dataObjects.Select(x => x.FirmId).ToHashSet();
            
            var orderIdsByFirm =
                _query.For<Order>()
                    .Where(x => firmIds.Contains(x.FirmId))
                    .Select(x => x.Id)
                    .Distinct();

            var firmAddressIds = dataObjects.Select(x => x.Id).ToHashSet();
            var orderIdsByUsage =
                _query.For<OrderPositionAdvertisement>()
                    .Where(x => firmAddressIds.Contains(x.FirmAddressId.Value))
                    .Select(x => x.OrderId)
                    .Distinct();
            
            var orderIds = orderIdsByFirm.Concat(orderIdsByUsage).ToHashSet();
            
            return new[] { new RelatedDataObjectOutdatedEvent(typeof(FirmAddress), typeof(Order), orderIds) };
        }
    }
}
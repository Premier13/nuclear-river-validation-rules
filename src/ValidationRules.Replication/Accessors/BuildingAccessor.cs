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
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class BuildingAccessor : IMemoryBasedDataObjectAccessor<Building>, IDataChangesHandler<Building>
    {
        private readonly IQuery _query;

        public BuildingAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<Building> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .OfType<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<BuildingDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            return dtos.Select(x => new Building
            {
                Id = x.Id,
                PurposeCode = x.PurposeCode,
            }).ToList();
        }

        public FindSpecification<Building> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.OfType<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<BuildingDto>().Select(x => x.Id).ToHashSet();
            return new FindSpecification<Building>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Building> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Building> dataObjects)=> Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Building> dataObjects)=> Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Building> dataObjects)
        {
            var buildingIds = dataObjects.Select(x => x.Id);

            var orderIds =
                (from fa in _query.For<FirmAddress>().Where(x => buildingIds.Contains(x.BuildingId.Value))
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => x.FirmAddressId.Value == fa.Id)
                select opa.OrderId)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(BranchOfficeOrganizationUnit), typeof(Order), orderIds)};
        }
    }
}
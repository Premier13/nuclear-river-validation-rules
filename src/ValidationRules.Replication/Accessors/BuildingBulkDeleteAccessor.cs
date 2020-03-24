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
    public sealed class BuildingBulkDeleteAccessor : IMemoryBasedDataObjectAccessor<BuildingBulkDelete>, IDataChangesHandler<BuildingBulkDelete>
    {
        private readonly IQuery _query;

        public BuildingBulkDeleteAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<BuildingBulkDelete> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var ids = commands
                .OfType<DeleteInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<BuildingBulkDeleteDto>()
                .Select(x => x.Id)
                .ToHashSet();

            return ids.Select(id => new BuildingBulkDelete
            {
                Id = id,
            }).ToList();
        }

        public FindSpecification<BuildingBulkDelete> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.OfType<DeleteInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<BuildingBulkDeleteDto>().Select(x => x.Id).ToHashSet();
            return new FindSpecification<BuildingBulkDelete>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BuildingBulkDelete> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BuildingBulkDelete> dataObjects)=> Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BuildingBulkDelete> dataObjects)=> Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<BuildingBulkDelete> dataObjects)
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
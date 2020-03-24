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
    public sealed class FirmAddressCategoryAccessor: IMemoryBasedDataObjectAccessor<FirmAddressCategory>, IDataChangesHandler<FirmAddressCategory>
    {
        private readonly IQuery _query;

        public FirmAddressCategoryAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<FirmAddressCategory> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .Cast<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<FirmAddressDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            var result = dtos
                .Where(Specs.Find.InfoRussia.FirmAddress.Active)
                .SelectMany(x => x.Categories.Select(categoryId => new FirmAddressCategory
                {
                    FirmAddressId = x.Id,
                    CategoryId = (long)categoryId,
                })).ToList();

            return result;
        }

        public FindSpecification<FirmAddressCategory> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmAddressDto>()
                .Select(x => x.Id).ToHashSet();
            return new FindSpecification<FirmAddressCategory>(x => ids.Contains(x.FirmAddressId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.FirmAddressId).ToHashSet();

            var orderIds =
                (from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                    from order in _query.For<Order>().Where(x => x.FirmId == firmAddress.FirmId)
                    select order.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(FirmAddressCategory), typeof(Order), orderIds)};
        }
    }
}
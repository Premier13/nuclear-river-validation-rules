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
    public sealed class FirmAccessor : IMemoryBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        private readonly IQuery _query;

        public FirmAccessor(IQuery query) => _query = query;

        public IReadOnlyCollection<Firm> GetDataObjects(IEnumerable<ICommand> commands)
        {
            var dtos = commands
                .Cast<SyncInMemoryDataObjectCommand>()
                .SelectMany(x => x.Dtos)
                .OfType<FirmDto>()
                .GroupBy(x => x.Id)
                .Select(x => x.Last());

            var result = dtos
                .Where(Specs.Find.InfoRussia.Firm.Active)
                .Select(x => new Firm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                }).ToList();

            return result;
        }

        public FindSpecification<Firm> GetFindSpecification(IEnumerable<ICommand> commands)
        {
            var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmDto>()
                .Select(x => x.Id).ToHashSet();
            return new FindSpecification<Firm>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
            => new [] {new DataObjectCreatedEvent(typeof(Firm), dataObjects.Select(x => x.Id)) };

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
            => new [] {new DataObjectUpdatedEvent(typeof(Firm), dataObjects.Select(x => x.Id)) };

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
            => new [] {new DataObjectDeletedEvent(typeof(Firm), dataObjects.Select(x => x.Id)) };

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Firm> dataObjects)
        {
            var firmIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds = _query.For<Order>()
                .Where(x => firmIds.Contains(x.FirmId))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(Firm), typeof(Order), orderIds)};
        }
    }
}
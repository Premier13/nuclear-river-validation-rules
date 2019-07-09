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
    public sealed class BranchOfficeOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<BranchOfficeOrganizationUnit>, IDataChangesHandler<BranchOfficeOrganizationUnit>
    {
        private readonly IQuery _query;

        public BranchOfficeOrganizationUnitAccessor(IQuery query) => _query = query;

        public IQueryable<BranchOfficeOrganizationUnit> GetSource() => _query
            .For<Erm::BranchOfficeOrganizationUnit>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new BranchOfficeOrganizationUnit
                {
                    Id = x.Id,
                    BranchOfficeId = x.BranchOfficeId
                });

        public FindSpecification<BranchOfficeOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<BranchOfficeOrganizationUnit>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
        {
            var branchOfficeOrganizationUnitIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds =
                from order in _query.For<Order>().Where(x => branchOfficeOrganizationUnitIds.Contains(x.BranchOfficeOrganizationUnitId.Value))
                select order.Id;

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(BranchOfficeOrganizationUnit), typeof(Order), orderIds.ToHashSet())};
        }
    }
}
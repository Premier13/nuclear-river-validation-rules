using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Tenancy
{
    public sealed class TenantAccessor<T> : IStorageBasedDataObjectAccessor<T>
        where T: ITenantEntity
    {
        private readonly IStorageBasedDataObjectAccessor<T> _implementation;
        private readonly Tenant _tenant;

        public TenantAccessor(IStorageBasedDataObjectAccessor<T> implementation, Tenant tenant)
        {
            _implementation = implementation;
            _tenant = tenant;
        }

        public IQueryable<T> GetSource()
            => _implementation.GetSource().Select(_tenant.ApplyToEntity<T>());

        public FindSpecification<T> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => _implementation.GetFindSpecification(commands);
    }
}

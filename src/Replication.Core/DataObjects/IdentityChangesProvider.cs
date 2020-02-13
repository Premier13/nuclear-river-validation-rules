using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.DataObjects
{
    public sealed class IdentityChangesProvider<TDataObject> : IChangesProvider<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;
        private readonly IEqualityComparer<TDataObject> _identityComparer;

        public IdentityChangesProvider(IQuery query,
                                       IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
                                       IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<TDataObject>();
        }

        public MergeResult<TDataObject> GetChanges(IReadOnlyCollection<ICommand> commands)
        {
            var specification = _storageBasedDataObjectAccessor.GetFindSpecification(commands);
            var source = new TransactionDecorator<TDataObject>(_storageBasedDataObjectAccessor.GetSource().WhereMatched(specification));
            var target = _query.For<TDataObject>().WhereMatched(specification);
            
            var result = MergeTool.Merge(source, target, _identityComparer);
            return result;
        }
    }
}
using System.Collections.Generic;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.DataObjects
{
    public sealed class IdentityInMemoryChangesProvider<TDataObject> : IChangesProvider<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IMemoryBasedDataObjectAccessor<TDataObject> _memoryBasedDataObjectAccessor;
        private readonly IEqualityComparer<TDataObject> _identityComparer;
        
        public IdentityInMemoryChangesProvider(IQuery query,
            IMemoryBasedDataObjectAccessor<TDataObject> memoryBasedDataObjectAccessor,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _memoryBasedDataObjectAccessor = memoryBasedDataObjectAccessor;
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<TDataObject>();
        }

        public MergeResult<TDataObject> GetChanges(IReadOnlyCollection<ICommand> commands)
        {
            var specification = _memoryBasedDataObjectAccessor.GetFindSpecification(commands);
            var source = _memoryBasedDataObjectAccessor.GetDataObjects(commands);
            var target = _query.For<TDataObject>().WhereMatched(specification);
            
            var result = MergeTool.Merge(source, target, _identityComparer);
            return result;
        }
    }
}
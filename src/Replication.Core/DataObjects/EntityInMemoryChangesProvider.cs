using System.Collections.Generic;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.DataObjects
{
    public sealed class EntityInMemoryChangesProvider<TDataObject> : IChangesProvider<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IMemoryBasedDataObjectAccessor<TDataObject> _memoryBasedDataObjectAccessor;
        private readonly TwoPhaseDataChangesDetector<TDataObject> _dataChangesDetector;

        public EntityInMemoryChangesProvider(IQuery query,
            IMemoryBasedDataObjectAccessor<TDataObject> memoryBasedDataObjectAccessor,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _memoryBasedDataObjectAccessor = memoryBasedDataObjectAccessor;
            _dataChangesDetector = new TwoPhaseDataChangesDetector<TDataObject>(equalityComparerFactory);
        }

        public MergeResult<TDataObject> GetChanges(IReadOnlyCollection<ICommand> commands)
        {
            var specification = _memoryBasedDataObjectAccessor.GetFindSpecification(commands);
            var source = _memoryBasedDataObjectAccessor.GetDataObjects(commands);
            var target = _query.For<TDataObject>().WhereMatched(specification);
            
            var result = _dataChangesDetector.DetectChanges(source, target);
            return result;
        }
    }
}
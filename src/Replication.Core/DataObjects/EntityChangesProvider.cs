using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.DataObjects
{
    public sealed class EntityChangesProvider<TDataObject> : IChangesProvider<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;
        private readonly TwoPhaseDataChangesDetector<TDataObject> _dataChangesDetector;

        public EntityChangesProvider(IQuery query,
                                     IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
                                     IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;
            _dataChangesDetector = new TwoPhaseDataChangesDetector<TDataObject>(equalityComparerFactory);
        }

        public MergeResult<TDataObject> GetChanges(IReadOnlyCollection<ICommand> commands)
        {
            var specification = _storageBasedDataObjectAccessor.GetFindSpecification(commands);
            var source = _storageBasedDataObjectAccessor.GetSource().WhereMatched(specification);
            var target = _query.For<TDataObject>().WhereMatched(specification);
            
            var result = _dataChangesDetector.DetectChanges(source, target);
            return result;
        }
    }
}
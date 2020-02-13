using System.Collections.Generic;
using NuClear.Replication.Core.Equality;

namespace NuClear.Replication.Core.DataObjects
{
    public class TwoPhaseDataChangesDetector<TDataObject>
    {
        private readonly IEqualityComparer<TDataObject> _identityComparer;
        private readonly IEqualityComparer<TDataObject> _completeComparer;

        public TwoPhaseDataChangesDetector(IEqualityComparerFactory equalityComparerFactory)
        {
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<TDataObject>();
            _completeComparer = equalityComparerFactory.CreateCompleteComparer<TDataObject>();
        }

        public MergeResult<TDataObject> DetectChanges(IEnumerable<TDataObject> source, IEnumerable<TDataObject> target)
        {
            var preResult = MergeTool.Merge(source, target, _completeComparer);
            
            // вторая фаза поможет найти объекты разные внутри но одинаковые по первичному ключу
            // таким образом мы нормально построим sql-инструкцию UPDATE 
            var result = MergeTool.Merge(preResult.Difference, preResult.Complement, _identityComparer);
            return result;
        }
    }
}
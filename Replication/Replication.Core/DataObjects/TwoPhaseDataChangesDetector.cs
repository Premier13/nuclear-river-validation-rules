﻿using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.DataObjects
{
    public class TwoPhaseDataChangesDetector<T>
    {
        private readonly DataChangesDetector<T> _dataChangesDetector;
        private readonly IEqualityComparer<T> _identityComparer;

        public TwoPhaseDataChangesDetector(
            MapToObjectsSpecProvider<T, T> sourceProvider,
            MapToObjectsSpecProvider<T, T> targetProvider,
            IEqualityComparer<T> identityComparer,
            IEqualityComparer<T> completeComparer)
        {
            _dataChangesDetector = new DataChangesDetector<T>(sourceProvider, targetProvider, completeComparer);
            _identityComparer = identityComparer;
        }

        public MergeResult<T> DetectChanges(FindSpecification<T> specification)
        {
            var preresult = _dataChangesDetector.DetectChanges(specification);
            return MergeTool.Merge(preresult.Difference, preresult.Complement, _identityComparer);
        }
    }
}
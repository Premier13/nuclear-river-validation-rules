using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;

namespace NuClear.Replication.Core.DataObjects
{
    // TODO: возможно стоит вместо этого всего расставить в actors using transaction, вроде как должно проканать
    public sealed class TransactionDecorator<TDataObject> : IEnumerable<TDataObject>
    {
        private readonly IEnumerable<TDataObject> _enumerable;

        public TransactionDecorator(IEnumerable<TDataObject> enumerable) => _enumerable = enumerable;

        public IEnumerator<TDataObject> GetEnumerator() => new EnumeratorDecorator(_enumerable);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class EnumeratorDecorator : IEnumerator<TDataObject>
        {
            private readonly IEnumerable<TDataObject> _enumerable;
            private IEnumerator<TDataObject> _enumerator;
            private TransactionScope _transaction;

            public EnumeratorDecorator(IEnumerable<TDataObject> enumerable)
                => _enumerable = enumerable;

            public void Dispose()
            {
                _transaction?.Dispose();
                _enumerator?.Dispose();
            }

            public bool MoveNext()
            {
                if (_transaction == null)
                {
                    _transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                    _enumerator = _enumerable.GetEnumerator();
                }

                return _enumerator.MoveNext();
            }

            public void Reset() => throw new NotSupportedException();

            public TDataObject Current => _enumerator.Current;

            object IEnumerator.Current => Current;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactWriters
{
    /// <summary>
    /// Используется для актуализации коллекций, группируемых по общему ключу.
    /// Ключ с пустой коллекцией приводит к удалению всех записей по этому ключу.
    /// </summary>
    public sealed class GroupWriter<TKey, TValue> : IEntityWriter where TValue : class
    {
        private readonly Expression<Func<TValue, TKey>> _groupKey;
        private readonly IRelationProvider<TValue> _relationProvider;
        private readonly IEqualityComparer<Group<TKey, TValue>> _keyComparer;

        public GroupWriter(
            IRelationProvider<TValue> relationProvider,
            Expression<Func<TValue, TKey>> groupKey)
        {
            _groupKey = groupKey;
            _relationProvider = relationProvider;
            _keyComparer = new GroupEqualityComparer(EqualityComparer<TKey>.Default);
        }

        public IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, ICollection data)
            => Write(dataConnection, (IReadOnlyCollection<Group<TKey, TValue>>) data);

        private IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection,
            IReadOnlyCollection<Group<TKey, TValue>> data)
        {
            var deduplicatedData = Instrumentation.Do(
                () => Deduplicate(data), "Writer deduplicate", typeof(TValue).Name);
            var (created, deleted) = Instrumentation.Do(
                () => CreateDataTable(dataConnection, deduplicatedData), "Writer bulkcopy", typeof(TValue).Name);
            var relations = Instrumentation.Do(
                () => CalculateRelations(dataConnection, created, deleted), "Writer relations", typeof(TValue).Name);
            Instrumentation.Do(
                () => WriteChanges(dataConnection, created, deleted), "Writer write", typeof(TValue).Name);

            return relations;
        }

        private IReadOnlyCollection<Group<TKey, TValue>> Deduplicate(IReadOnlyCollection<Group<TKey, TValue>> data)
        {
            var deduplicatedData = new HashSet<Group<TKey, TValue>>(data.Count, _keyComparer);
            deduplicatedData.UnionWith(data.Reverse());
            return deduplicatedData;
        }

        private IReadOnlyCollection<RelationRecord> CalculateRelations(
            DataConnection dataConnection,
            IQueryable<TValue> created,
            IQueryable<TValue> deleted)
        {
            if (_relationProvider == null)
                return Array.Empty<RelationRecord>();

            return _relationProvider.GetRelations(dataConnection, created, deleted);
        }

        private (IQueryable<TValue>, IQueryable<TValue>) CreateDataTable(
            DataConnection dataConnection,
            IReadOnlyCollection<Group<TKey, TValue>> data)
        {
            var received = dataConnection.CreateTable<TValue>($"#{typeof(TValue).Name}");
            received.BulkCopy(data.SelectMany(x => x.Values));

            var keysTable = dataConnection.CreateTable<TKey>($"#{typeof(TValue).Name}Key");
            keysTable.BulkCopy(data.Select(x => x.Key));
            var stored = keysTable
                .Join(dataConnection.GetTable<TValue>(), x => x, _groupKey, (key, stored) => stored);

            var created = received.Except(stored);
            var deleted = stored.Except(received);

            return (created, deleted);
        }

        private void WriteChanges(DataConnection dataConnection, IQueryable<TValue> created, IQueryable<TValue> deleted)
        {
            dataConnection.GetTable<TValue>().Delete(x => deleted.Contains(x));
            created.MergeInto(dataConnection.GetTable<TValue>())
                .OnTargetKey()
                .InsertWhenNotMatched()
                .Merge();
        }

        private sealed class GroupEqualityComparer : IEqualityComparer<Group<TKey, TValue>>
        {
            private readonly IEqualityComparer<TKey> _keyEqualityComparer;

            public GroupEqualityComparer(IEqualityComparer<TKey> keyEqualityComparer)
                => _keyEqualityComparer = keyEqualityComparer;

            public bool Equals(Group<TKey, TValue> x, Group<TKey, TValue> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return _keyEqualityComparer.Equals(x.Key, y.Key);
            }

            public int GetHashCode(Group<TKey, TValue> obj)
                => _keyEqualityComparer.GetHashCode(obj.Key);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing.Writers
{
    /// <summary>
    /// Используется для актуализации идентифицируемых (в шине и базе) объектов.
    /// Создаёт новые записи, актуализирует изменившиеся и удаляет помеченные IsDeleted.
    /// </summary>
    public sealed class EntityWriter<TKey, TValue> : IEntityWriter where TValue : class
    {
        private readonly Expression<Func<TValue,TKey>> _entityKey;
        private readonly IRelationProvider<TValue> _relationProvider;
        private readonly IEqualityComparer<TValue> _entityComparer;

        public EntityWriter(
            IRelationProvider<TValue> relationProvider,
            Expression<Func<TValue, TKey>> entityKey)
        {
            _entityKey = entityKey;
            _relationProvider = relationProvider;
            _entityComparer = new EntityKeyComparer(entityKey.Compile(), EqualityComparer<TKey>.Default);
        }

        public IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, ICollection data)
            => Write(dataConnection, (IReadOnlyCollection<TValue>) data);

        private IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IReadOnlyCollection<TValue> data)
        {
            var deduplicatedData = Instrumentation.Do(
                () => Deduplicate(data), "Writer deduplicate", typeof(TValue).Name);
            var actual = Instrumentation.Do(
                () => CreateDataTable(dataConnection, deduplicatedData), "Writer bulkcopy", typeof(TValue).Name);
            var relations = Instrumentation.Do(
                () => CalculateRelations(dataConnection, actual), "Writer relations", typeof(TValue).Name);
            Instrumentation.Do(
                () => WriteChanges(dataConnection, actual), "Writer write", typeof(TValue).Name);

            return relations;
        }

        private IReadOnlyCollection<TValue> Deduplicate(IReadOnlyCollection<TValue> data)
        {
            var deduplicatedData = new HashSet<TValue>(data.Count, _entityComparer);
            deduplicatedData.UnionWith(data.Reverse());
            return deduplicatedData;
        }

        private void WriteChanges(
            DataConnection dataConnection,
            ITable<TValue> actual)
        {
            var descriptor = dataConnection.MappingSchema.GetEntityDescriptor(typeof(TValue));

            var mergeCommand = actual.MergeInto(dataConnection.GetTable<TValue>())
                .OnTargetKey()
                .InsertWhenNotMatched();

            if (IsDeletable())
                mergeCommand = mergeCommand.DeleteWhenMatchedAnd((target, source) => ((IDeletable) source).IsDeleted);

            if (HasUpdatableProperties(descriptor))
                mergeCommand = mergeCommand.UpdateWhenMatched();

            mergeCommand.Merge();
        }

        private IReadOnlyCollection<RelationRecord> CalculateRelations(
            DataConnection dataConnection,
            IQueryable<TValue> actual)
        {
            if (_relationProvider == null)
                return Array.Empty<RelationRecord>();

            // todo: ждём в третьей версии фичу 'output': https://github.com/linq2db/linq2db/pull/1703
            // это позволит проще определять, какие записи изменились поменялось.
            var updated = dataConnection.GetTable<TValue>().DefaultIfEmpty()
                .Join(actual, _entityKey, _entityKey, (stored, received) => new {stored, received})
                .Where(x => x.stored != x.received);

            Debug.WriteLine(updated.Select(x => x.received).ToString());
            Debug.WriteLine(updated.Select(x => x.stored).ToString());
            Debugger.Break();

            var relations = _relationProvider.GetRelations(
                dataConnection,
                updated.Select(x => x.received),
                updated.Select(x => x.stored));

            return relations;
        }

        private ITable<TValue> CreateDataTable(
            IDataContext context,
            IEnumerable<TValue> data)
        {
            var tempTable = context.CreateTable<TValue>($"#{typeof(TValue).Name}");
            tempTable.BulkCopy(data);

            return tempTable;
        }

        private bool IsDeletable()
            => typeof(IDeletable).IsAssignableFrom(typeof(TValue));

        private bool HasUpdatableProperties(EntityDescriptor descriptor)
            => descriptor.Columns.Any(x => !x.IsIdentity && !x.IsPrimaryKey && !x.SkipOnUpdate);

        private class EntityKeyComparer : EqualityComparer<TValue>
        {
            private readonly Func<TValue, TKey> _keyProjection;
            private readonly EqualityComparer<TKey> _keyComparer;

            public EntityKeyComparer(Func<TValue, TKey> keyProjection, EqualityComparer<TKey> keyComparer)
                => (_keyProjection, _keyComparer) = (keyProjection, keyComparer);

            public override bool Equals(TValue x, TValue y)
                => ReferenceEquals(x, y)
                    || x != null && y != null && _keyComparer.Equals(_keyProjection(x), _keyProjection(y));

            public override int GetHashCode(TValue obj)
                => _keyComparer.GetHashCode(_keyProjection(obj));
        }
    }
}

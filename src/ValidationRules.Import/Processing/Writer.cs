using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Writer
    {
        private static readonly TransactionOptions TransactionOptions
            = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted};

        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly IDictionary<Type, IEntityWriter> _writerByEntityType;
        private readonly bool _ignoreRelations;

        public Writer(DataConnectionFactory dataConnectionFactory, bool ignoreRelations)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _writerByEntityType = new Dictionary<Type, IEntityWriter>();
            _ignoreRelations = ignoreRelations;
        }

        public FluentBuilder<TValue> Entity<TValue>() where TValue : class
            => new FluentBuilder<TValue>(this, _ignoreRelations);

        public void Write(IReadOnlyDictionary<Type, ICollection> data, CancellationToken token)
        {
            lock (_writerByEntityType)
            {
                var allRelations = new HashSet<RelationRecord>(10000, new RelationRecord.EqualityComparer());

                using var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionOptions);
                using var dataConnection = _dataConnectionFactory.Create();
                foreach (var (key, value) in data)
                {
                    if (value.Count == 0)
                        continue;

                    if (!_writerByEntityType.TryGetValue(key, out var writer))
                        throw new Exception($"No relation provider for type '{key.FullName}' was added.");

                    if (token.IsCancellationRequested)
                        return;

                    var entityRelations = writer.Write(dataConnection, value);

                    if (token.IsCancellationRequested)
                        return;

                    allRelations.UnionWith(entityRelations);
                }

                if (token.IsCancellationRequested)
                    return;

                WriteEvents(dataConnection, allRelations);

                transaction.Complete();
                Log.Info("Write transaction completed", new object());
            }
        }

        private void Configure<TValue, TKey>(
            IRelationProvider<TValue> relationProvider,
            Expression<Func<TValue, TKey>> keyExpression) where TValue : class
        {
            lock (_writerByEntityType)
            {
                _writerByEntityType[typeof(TValue)] = new EntityWriter<TKey, TValue>(relationProvider, keyExpression);
            }
        }

        private static void WriteEvents(DataConnection dataConnection, IReadOnlyCollection<RelationRecord> relations)
        {
            const string flow = "9BD1C845-2574-4003-8722-8A55B1D4AE38";
            const string eventTemplate = "<event type='RelatedDataObjectOutdatedEvent'>" +
                "<type>{0}</type>" +
                "<relatedType>{1}</relatedType>" +
                "<relatedId>{2}</relatedId>" +
                "</event>";

            dataConnection.BulkCopy(relations.Select(x => new EventRecord
            {
                Flow = flow,
                Content = string.Format(eventTemplate, x.Type, x.RelatedType, x.RelatedId),
            }));
        }

        public class FluentBuilder<TValue> where TValue : class
        {
            private readonly Writer _writer;
            private readonly bool _ignoreRelationsProvider;
            private readonly IRelationProvider<TValue> _provider;

            public FluentBuilder(Writer writer, bool ignoreRelationsProvider)
            {
                _writer = writer;
                _ignoreRelationsProvider = ignoreRelationsProvider;
            }

            private FluentBuilder(Writer writer, bool ignoreRelationsProvider, IRelationProvider<TValue> provider)
                => (_writer, _ignoreRelationsProvider, _provider) = (writer, ignoreRelationsProvider, provider);

            public FluentBuilder<TValue> HasRelationsProvider(IRelationProvider<TValue> provider)
                => new FluentBuilder<TValue>(_writer, _ignoreRelationsProvider, provider);

            public void HasKey<TKey>(Expression<Func<TValue, TKey>> keyExpression)
            {
                var provider = _ignoreRelationsProvider
                    ? null
                    : (_provider ?? throw new InvalidOperationException($"Relations provider must be set."));

                _writer.Configure(provider, keyExpression);
            }
        }

        private interface IEntityWriter
        {
            IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable data);
        }

        private class EntityWriter<TKey, TEntity> : IEntityWriter where TEntity : class
        {
            private readonly IRelationProvider<TEntity> _relationProvider;
            private readonly Expression<Func<TEntity, TKey>> _entityKey;

            public EntityWriter(
                IRelationProvider<TEntity> relationProvider,
                Expression<Func<TEntity, TKey>> entityKey)
            {
                _relationProvider = relationProvider;
                _entityKey = entityKey;
            }

            public IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable data)
                => Write(dataConnection, (IEnumerable<TEntity>) data);

            private IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable<TEntity> data)
            {
                var tempTable = CreateDataTable(dataConnection, data);
                var relations = CalculateRelations(dataConnection, tempTable);
                WriteChanges(dataConnection, tempTable);

                return relations;
            }

            private void WriteChanges(
                DataConnection dataConnection,
                IQueryable<TEntity> tempTable)
            {
                var descriptor = dataConnection.MappingSchema.GetEntityDescriptor(typeof(TEntity));

                // todo: здесь нет удаления из фактов. пока кажется, что это не большая проблема,
                //       можно даже вынести чистку в отдельную задачу.
                var mergeTimer = Stopwatch.StartNew();
                var mergeCommand = tempTable.MergeInto(dataConnection.GetTable<TEntity>())
                    .OnTargetKey()
                    .InsertWhenNotMatched();

                if (IsDeletable())
                    mergeCommand =
                        mergeCommand.DeleteWhenMatchedAnd((target, source) => ((IDeletable) source).IsDeleted);

                // todo: что-то такое может потребоваться, если в потоке будут снимки состояния, не включающие удалённые объекты (т.е. нам самим придётся понимать, что удалено)
                // .DeleteWhenNotMatchedBySourceAnd(x => tempTable.Contains(y => x.AggregateId == y.AggregateId))

                if (HasUpdatableProperties(descriptor))
                    mergeCommand = mergeCommand.UpdateWhenMatched();

                mergeCommand.Merge();
                mergeTimer.Stop();

                Log.Debug("Flush merge records", new {typeof(TEntity).Name, Time = mergeTimer.Elapsed});
            }

            private IReadOnlyCollection<RelationRecord> CalculateRelations(
                DataConnection dataConnection,
                IQueryable<TEntity> updated)
            {
                if (_relationProvider == null)
                    return Array.Empty<RelationRecord>();

                // todo: здесь нет определения записей, которые изменились реально.
                //       это можно добавить отдельными запросами, оно несколько утяжелит обработку потока.
                //       а вообще ждём в третьей версии фичу 'output': https://github.com/linq2db/linq2db/pull/1703
                // todo: если для merge будет использоваться некий AggregateKey, то outdated будет некорректен. Пока всё ок.
                var relationsTimer = Stopwatch.StartNew();
                var outdated =
                    updated.Join(dataConnection.GetTable<TEntity>(), _entityKey, _entityKey, (temp, stored) => stored);
                var relations = _relationProvider.GetRelations(dataConnection, updated, outdated);
                relationsTimer.Stop();

                Log.Debug("Flush calculate relations", new {typeof(TEntity).Name, Time = relationsTimer.Elapsed});

                return relations;
            }

            private ITable<TEntity> CreateDataTable(
                DataConnection dataConnection,
                IEnumerable<TEntity> data)
            {
                var bulkCopyTimer = Stopwatch.StartNew();
                var tempTable = dataConnection.CreateTable<TEntity>($"#{typeof(TEntity).Name}");
                tempTable.BulkCopy(data);
                bulkCopyTimer.Stop();

                Log.Debug("Flush bulk copy", new {typeof(TEntity).Name, Time = bulkCopyTimer.Elapsed});

                return tempTable;
            }

            private bool IsDeletable()
                => typeof(IDeletable).IsAssignableFrom(typeof(TEntity));

            private bool HasUpdatableProperties(EntityDescriptor descriptor)
                => descriptor.Columns.Any(x => !x.IsIdentity && !x.IsPrimaryKey && !x.SkipOnUpdate);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.SqlStore
{
    public sealed class DataWriter
    {
        private static readonly TransactionOptions TransactionOptions
            = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted};

        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly Action<DataConnection, IReadOnlyCollection<RelationRecord>> _eventFactory;
        private readonly IDictionary<Type, IWriter> _writers;

        public DataWriter(DataConnectionFactory dataConnectionFactory,
            Action<DataConnection, IReadOnlyCollection<RelationRecord>> eventFactory)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _eventFactory = eventFactory;
            _writers = new Dictionary<Type, IWriter>();
        }

        public void Add<TValue, TKey>(IRelationProvider<TValue> relationProvider,
            Expression<Func<TValue, TKey>> keyExpression) where TValue : class
        {
            _writers.Add(typeof(TValue), new Writer<TValue, TKey>(relationProvider, keyExpression));
        }

        public void Write(IReadOnlyDictionary<Type, IEnumerable> data, CancellationToken token)
        {
            lock (_writers)
            {
                var allRelations = new HashSet<RelationRecord>(10000, new RelationRecord.EqualityComparer());

                using var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionOptions);
                using var dataConnection = _dataConnectionFactory.Create();
                foreach (var (key, value) in data)
                {
                    if (!_writers.TryGetValue(key, out var writer))
                        throw new Exception($"No relation provider for type '{key.FullName}' was added.");

                    if(token.IsCancellationRequested)
                        return;

                    var entityRelations = writer.Write(dataConnection, value);

                    if(token.IsCancellationRequested)
                        return;

                    allRelations.UnionWith(entityRelations);
                }

                if(token.IsCancellationRequested)
                    return;

                _eventFactory.Invoke(dataConnection, allRelations);

                transaction.Complete();
            }
        }

        private interface IWriter
        {
            IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable data);
        }

        private class Writer<TValue, TKey> : IWriter where TValue : class
        {
            private readonly IRelationProvider<TValue> _relationProvider;
            private readonly Expression<Func<TValue, TKey>> _entityKey;

            public Writer(IRelationProvider<TValue> relationProvider, Expression<Func<TValue, TKey>> entityKey)
            {
                _relationProvider = relationProvider;
                _entityKey = entityKey;
            }

            public IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable data)
                => Write(dataConnection, (IEnumerable<TValue>) data);

            private IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, IEnumerable<TValue> data)
            {
                var tempTable = dataConnection.CreateTable<TValue>($"#{typeof(TValue).Name}");
                tempTable.BulkCopy(data);

                // todo: здесь нет определения записей, которые изменились реально.
                //       это можно добавить отдельными запросами, оно несколько утяжелит обработку потока.
                //       а вообще ждём в третьей версии фичу 'output': https://github.com/linq2db/linq2db/pull/1703
                var outdated =
                    tempTable.Join(dataConnection.GetTable<TValue>(), _entityKey, _entityKey, (temp, stored) => stored);

                var relations = _relationProvider.GetRelations(dataConnection, tempTable)
                    .Union(_relationProvider.GetRelations(dataConnection, outdated))
                    .Distinct() // нужен ли он тут? снижаем нугрузку на локальную память, увеличивает на sql сервер.
                    .ToList();

                // todo: здесь нет удаления из фактов. пока кажется, что это не большая проблема,
                //       можно даже вынести чистку в отдельную задачу.
                tempTable.MergeInto(dataConnection.GetTable<TValue>())
                    .OnTargetKey()
                    .UpdateWhenMatched()
                    .InsertWhenNotMatched()
                    .Merge();

                Log.Debug("Entities flushed", new {typeof(TValue).Name, Count = tempTable.Count()});

                return relations;
            }
        }
    }
}

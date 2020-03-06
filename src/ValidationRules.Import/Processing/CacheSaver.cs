using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class CacheSaver
    {
        private static readonly TransactionOptions TransactionOptions
            = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted};

        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly IDictionary<Type, IEntityWriter> _writerByEntityType;

        public CacheSaver(DataConnectionFactory dataConnectionFactory)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _writerByEntityType = new Dictionary<Type, IEntityWriter>();
        }

        public void Add(Type dataType, IEntityWriter writer)
        {
            lock (_writerByEntityType)
            {
                _writerByEntityType[dataType] = writer;
            }
        }

        public void Write(IReadOnlyDictionary<Type, ICollection> data, CancellationToken token)
        {
            lock (_writerByEntityType)
            {
                var allRelations = new HashSet<RelationRecord>(10000, new RelationRecord.EqualityComparer());
                var summary = new Dictionary<string, int>();

                using var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionOptions);
                using var dataConnection = _dataConnectionFactory.Create();
                foreach (var (key, value) in data)
                {
                    if (value.Count == 0)
                        continue;

                    summary.Add(key.Name, value.Count);

                    if (!_writerByEntityType.TryGetValue(key, out var writer))
                        throw new Exception($"No relation provider for type '{key.FullName}' was added.");

                    if (token.IsCancellationRequested)
                        return;

                    var entityRelations = writer.Write(dataConnection, value);

                    Log.Debug("Write entities completed",
                        new {Type = key.Name, value.Count, RelationCount = entityRelations.Count});

                    if (token.IsCancellationRequested)
                        return;

                    allRelations.UnionWith(entityRelations);
                }

                if (token.IsCancellationRequested)
                    return;

                WriteEvents(dataConnection, allRelations);

                transaction.Complete();
                Log.Info("Write transaction completed", summary);
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
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Relations;
using NuClear.ValidationRules.Import.SqlStore;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class ProducerConfiguration
    {
        private readonly ProducerCache _cache;
        private readonly DataWriter _writer;

        public ProducerConfiguration(ProducerCache cache, DataWriter writer)
        {
            _cache = cache;
            _writer = writer;
        }

        public void Add<TValue, TKey>(IRelationProvider<TValue> provider, Expression<Func<TValue, TKey>> key)
            where TValue : class
        {
            _cache.Register(key);
            _writer.Add(provider, key);
        }
    }

    public sealed class Producer
    {
        private static readonly int MaxCacheSize = 100000;
        private static readonly TimeSpan MaxCacheAge = TimeSpan.FromSeconds(10);

        private readonly ProducerCache _cache;
        private readonly DataWriter _writer;
        private readonly Task _backgroundTask;
        private readonly CancellationTokenSource _backgroundTaskCancellation;

        private Producer(ProducerCache cache, DataWriter writer)
        {
            _cache = cache;
            _writer = writer;

            _backgroundTaskCancellation = new CancellationTokenSource();
            _backgroundTask = new Task(
                () => FlushLoop(_backgroundTaskCancellation.Token),
                _backgroundTaskCancellation.Token);
        }

        public static Producer Create(
            DataConnectionFactory dataConnectionFactory,
            Action<ProducerConfiguration> configuration,
            Action<DataConnection, IReadOnlyCollection<RelationRecord>> eventFactory)
        {
            // eventFactory - временный костыль, чтобы оставить эту сборку без привязки к типам

            var cache = new ProducerCache(MaxCacheSize, MaxCacheAge);
            var dataWriter = new DataWriter(dataConnectionFactory, eventFactory);

            configuration.Invoke(new ProducerConfiguration(cache, dataWriter));

            var producer = new Producer(cache, dataWriter);
            producer._backgroundTask.Start();
            return producer;
        }

        public void InsertOrUpdate(IEnumerable entities)
        {
            if (entities == null)
                return;

            _cache.InsertOrUpdate(entities);
        }

        public void ThrowIfBackgroundFailed()
        {
            if(_backgroundTask.Exception != null)
                throw _backgroundTask.Exception;
        }

        private void FlushLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(100);
                if (_cache.ConsumeRequired())
                {
                    var data = _cache.Consume();
                    _writer.Write(data, token);
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _backgroundTaskCancellation.Cancel();
                _backgroundTask.Wait();
            }
            catch
            {
                // AggregateException
                // OperationCancelledException
            }
        }
    }
}

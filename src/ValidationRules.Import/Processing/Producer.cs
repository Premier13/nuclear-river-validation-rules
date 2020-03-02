using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Producer
    {
        private static readonly int MaxCacheSize = 100000;
        private static readonly TimeSpan MaxCacheAge = TimeSpan.FromSeconds(10);

        private readonly Cache _cache;
        private readonly Writer _writer;
        private readonly Task _backgroundTask;
        private readonly CancellationTokenSource _backgroundTaskCancellation;
        private readonly AutoResetEvent _cacheConsumed;

        private Producer(Cache cache, Writer writer)
        {
            _cache = cache;
            _writer = writer;
            _cacheConsumed = new AutoResetEvent(false);

            _backgroundTaskCancellation = new CancellationTokenSource();
            _backgroundTask = new Task(
                () => FlushLoop(_backgroundTaskCancellation.Token),
                _backgroundTaskCancellation.Token);
        }

        public static Producer Create(
            DataConnectionFactory dataConnectionFactory,
            IReadOnlyCollection<IEntityConfiguration> configurations)
        {
            var cache = new Cache(MaxCacheSize, MaxCacheAge);
            var dataWriter = new Writer(dataConnectionFactory);

            foreach (var configuration in configurations)
            {
                configuration.Apply(cache);
                configuration.Apply(dataWriter);
            }

            var producer = new Producer(cache, dataWriter);
            producer._backgroundTask.Start();
            return producer;
        }

        public void InsertOrUpdate(IEnumerable entities)
        {
            if (entities == null)
                return;

            _cache.InsertOrUpdate(entities);

            // Если кеш заполнен, ждём пока данные оттуда заберут.
            // Ждём ограниченное время, лучше превысить размер, чем завесить основной поток.
            if (_cache.ConsumeRequired())
            {
                var throttlingTimer = Stopwatch.StartNew();
                _cacheConsumed.WaitOne(TimeSpan.FromSeconds(10));
                throttlingTimer.Stop();
                Log.Warn("Producer throttling", new {Time = throttlingTimer.Elapsed});
            }
        }

        public void ThrowIfBackgroundFailed()
        {
            if (_backgroundTask.Exception != null)
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
                    _cacheConsumed.Set();
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

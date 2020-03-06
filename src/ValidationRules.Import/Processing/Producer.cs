using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Producer
    {
        private readonly TimeSpan _flushInterval;

        private readonly Cache _cache;
        private readonly CacheSaver _cacheSaver;
        private readonly Task _backgroundTask;
        private readonly CancellationTokenSource _backgroundTaskCancellation;

        private DateTime _lastConsumed;

        public Producer(Cache cache, CacheSaver cacheSaver, TimeSpan flushInterval)
        {
            _cache = cache;
            _cacheSaver = cacheSaver;
            _lastConsumed = DateTime.UtcNow;
            _flushInterval = flushInterval;

            _backgroundTaskCancellation = new CancellationTokenSource();
            _backgroundTask = new Task(
                () => FlushLoop(_backgroundTaskCancellation.Token),
                _backgroundTaskCancellation.Token);
            _backgroundTask.Start();
        }

        public void InsertOrUpdate(IEnumerable entities)
        {
            if (entities == null)
                return;

            _cache.InsertOrUpdate(entities);
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
                if (_cache.HasEnoughData || _lastConsumed + _flushInterval < DateTime.UtcNow)
                {
                    var data = _cache.Consume();
                    _lastConsumed = DateTime.UtcNow;
                    _cacheSaver.Write(data, token);
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

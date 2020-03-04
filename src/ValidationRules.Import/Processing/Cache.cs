using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Cache
    {
        private readonly int _maxCount;
        private readonly IDictionary<Type, IEntityCache> _cacheByEntityType;
        private readonly AutoResetEvent _cacheConsumed;
        private int _count;

        public Cache(int maxCount)
        {
            _maxCount = maxCount;
            _cacheConsumed = new AutoResetEvent(false);
            _cacheByEntityType = new Dictionary<Type, IEntityCache>(50);
            _count = 0;
        }

        public bool HasEnoughData => _count > _maxCount;

        public void InsertOrUpdate(IEnumerable entities)
        {
            // Если кеш заполнен, ждём пока данные оттуда заберут.
            // Ждём ограниченное время, лучше превысить размер, чем завесить основной поток.
            if (HasEnoughData)
            {
                var throttlingTimer = Stopwatch.StartNew();
                _cacheConsumed.WaitOne(TimeSpan.FromSeconds(10));
                throttlingTimer.Stop();
                Log.Warn("Consume throttling", new {Time = throttlingTimer.Elapsed});
            }

            lock (_cacheByEntityType)
            {
                foreach (var entity in entities)
                {
                    if (!_cacheByEntityType.TryGetValue(entity.GetType(), out var cache))
                        _cacheByEntityType.Add(entity.GetType(), cache = CreateCache(entity.GetType()));
                    cache.InsertOrUpdate(entity);
                    _count++;
                }
            }
        }

        public IReadOnlyDictionary<Type, ICollection> Consume()
        {
            lock (_cacheByEntityType)
            {
                _count = 0;
                return _cacheByEntityType.ToDictionary(x => x.Key, x => x.Value.Consume());
            }
        }

        public void Entity<TEntity>() where TEntity : class
        {
        }

        private static IEntityCache CreateCache(Type type)
            => (IEntityCache)Activator.CreateInstance(typeof(EntityCache<>).MakeGenericType(type));

        private interface IEntityCache
        {
            void InsertOrUpdate(object entity);
            ICollection Consume();
        }

        private sealed class EntityCache<TValue> : IEntityCache where TValue : class
        {
            private List<TValue> _data;

            public EntityCache()
            {
                _data = new List<TValue>();
            }

            public void InsertOrUpdate(object entity)
                => _data.Add((TValue)entity);

            public ICollection Consume()
            {
                // Блокировка не требуется, она есть выше по стеку.
                var values = _data;
                _data = new List<TValue>();
                return values;
            }
        }
    }
}

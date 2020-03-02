using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Cache
    {
        private readonly int _cacheSize;
        private readonly TimeSpan _maxAge;
        private readonly IDictionary<Type, IEntityCache> _cacheByEntityType;
        private DateTime _lastConsumed;

        public Cache(int cacheSize, TimeSpan maxAge)
        {
            _cacheSize = cacheSize;
            _maxAge = maxAge;
            _cacheByEntityType = new Dictionary<Type, IEntityCache>(cacheSize);
            _lastConsumed = DateTime.UtcNow;
        }

        public FluentBuilder<TEntity> Entity<TEntity>() where TEntity : class => new FluentBuilder<TEntity>(this);

        public void InsertOrUpdate(IEnumerable entities)
        {
            lock (_cacheByEntityType)
            {
                foreach (var entity in entities)
                    _cacheByEntityType[entity.GetType()].InsertOrUpdate(entity);
            }
        }

        public bool ConsumeRequired()
        {
            lock (_cacheByEntityType)
            {
                return _cacheByEntityType.Values.Sum(x => x.Count) > _cacheSize || (DateTime.UtcNow - _lastConsumed) > _maxAge;
            }
        }

        public IReadOnlyDictionary<Type, ICollection> Consume()
        {
            lock (_cacheByEntityType)
            {
                _lastConsumed = DateTime.UtcNow;
                return _cacheByEntityType.ToDictionary(x => x.Key, x => x.Value.Consume());
            }
        }

        private void Configure<TEntity, TKey>(Expression<Func<TEntity, TKey>> keyExpression) where TEntity : class
        {
            // в принципе, ключ в кеше нужен только для дедупликации объектов в памяти, в процессе потребения их из потока между записями в базу.
            // сейчас код, записывающий изменения (BulkCopy) полагается на отсутствие дублей.
            // с другой стороны можно перенести дедупликацию туда (это будет отчасти даже правильно) - ключ там уже есть, его добавлять не нужно.
            // при этом из кеша этот ключ можно будет убрать вообще. но это увеличит нагрузку на память.
            lock (_cacheByEntityType)
            {
                _cacheByEntityType[typeof(TEntity)] = new EntityCache<TKey, TEntity>(keyExpression);
            }
        }

        public sealed class FluentBuilder<TEntity> where TEntity : class
        {
            private readonly Cache _cache;

            public FluentBuilder(Cache cache)
                => _cache = cache;

            public void HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
            {
                _cache.Configure(keyExpression);
            }
        }

        private interface IEntityCache
        {
            int Count { get; }
            void InsertOrUpdate(object entity);
            ICollection Consume();
        }

        private sealed class EntityCache<TKey, TValue> : IEntityCache where TValue : class
        {
            private readonly IDictionary<TKey, TValue> _data;
            private readonly Func<TValue, TKey> _keyFunction;

            public EntityCache(Expression<Func<TValue, TKey>> keyExpression)
            {
                _keyFunction = keyExpression.Compile();
                _data = new Dictionary<TKey, TValue>(EqualityComparer<TKey>.Default);
            }

            public int Count => _data.Count;

            public void InsertOrUpdate(object entity)
                => InsertOrUpdate((TValue) entity);

            private void InsertOrUpdate(TValue entity)
                => _data[_keyFunction.Invoke(entity)] = entity;

            public ICollection Consume()
            {
                var values = new List<TValue>(_data.Values);
                _data.Clear();
                return values;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class ProducerCache
    {
        private readonly int _cacheSize;
        private readonly TimeSpan _maxAge;
        private readonly IDictionary<Type, IEntityCache> _dictionary;
        private DateTime _lastConsumed;

        public ProducerCache(int cacheSize, TimeSpan maxAge)
        {
            _cacheSize = cacheSize;
            _maxAge = maxAge;
            _dictionary = new Dictionary<Type, IEntityCache>(cacheSize);
            _lastConsumed = DateTime.UtcNow;
        }

        public void InsertOrUpdate(IEnumerable entities)
        {
            lock (_dictionary)
            {
                foreach (var entity in entities)
                    _dictionary[entity.GetType()].InsertOrUpdate(entity);
            }
        }

        public bool ConsumeRequired()
        {
            lock (_dictionary)
            {
                return _dictionary.Values.Sum(x => x.Count) > _cacheSize || (DateTime.UtcNow - _lastConsumed) > _maxAge;
            }
        }

        public IReadOnlyDictionary<Type, IEnumerable> Consume()
        {
            lock (_dictionary)
            {
                _lastConsumed = DateTime.UtcNow;
                return _dictionary.ToDictionary(x => x.Key, x => x.Value.Consume());
            }
        }

        public void Register<TEntity, TKey>(Expression<Func<TEntity, TKey>> keyExpression) where TEntity : class
        {
            lock (_dictionary)
            {
                _dictionary[typeof(TEntity)] = new EntityCache<TKey, TEntity>(keyExpression);
            }
        }

        private interface IEntityCache
        {
            int Count { get; }
            void InsertOrUpdate(object entity);
            IEnumerable Consume();
        }

        private sealed class EntityCache<TKey, TValue> : IEntityCache where TValue : class
        {
            private readonly IDictionary<TKey, TValue> _cache;
            private readonly Func<TValue, TKey> _keyFunction;

            public EntityCache(Expression<Func<TValue, TKey>> keyExpression)
            {
                _keyFunction = keyExpression.Compile();
                _cache = new Dictionary<TKey, TValue>(EqualityComparer<TKey>.Default);
            }

            public int Count => _cache.Count;

            public void InsertOrUpdate(object entity)
                => InsertOrUpdate((TValue) entity);

            private void InsertOrUpdate(TValue entity)
                => _cache[_keyFunction.Invoke(entity)] = entity;

            public IEnumerable Consume()
            {
                var values = new List<TValue>(_cache.Values);
                _cache.Clear();
                return values;
            }
        }
    }
}

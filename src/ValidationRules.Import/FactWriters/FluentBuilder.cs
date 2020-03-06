using System;
using System.Linq.Expressions;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactWriters
{
    public static class FluentBuilderExtensions
    {
        public static FluentBuilder<TValue> Entity<TValue>(this CacheSaver cacheSaver) where TValue : class
            => new FluentBuilder<TValue>(cacheSaver);
    }

    public sealed class FluentBuilder<TValue> where TValue : class
    {
        private readonly CacheSaver _cacheSaver;
        private readonly IRelationProvider<TValue> _provider;

        public FluentBuilder(CacheSaver cacheSaver)
        {
            _cacheSaver = cacheSaver;
        }

        private FluentBuilder(CacheSaver cacheSaver, IRelationProvider<TValue> provider)
            => (_cacheSaver, _provider) = (cacheSaver, provider);

        public FluentBuilder<TValue> HasRelationsProvider(IRelationProvider<TValue> provider)
            => new FluentBuilder<TValue>(_cacheSaver, provider);

        public void HasKey<TKey>(Expression<Func<TValue, TKey>> keyExpression)
            => _cacheSaver.Add(typeof(TValue), new EntityWriter<TKey, TValue>(_provider, keyExpression));

        public void HasGroupKey<TKey>(Expression<Func<TValue, TKey>> keyExpression)
            => _cacheSaver.Add(typeof(Group<TKey, TValue>), new GroupWriter<TKey, TValue>(_provider, keyExpression));
    }
}

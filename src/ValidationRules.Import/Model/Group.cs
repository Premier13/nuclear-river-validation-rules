using System.Collections.Generic;

namespace NuClear.ValidationRules.Import.Model
{
    public static class Group
    {
        public static Group<TKey, TValue> Create<TKey, TValue>(TKey key, IReadOnlyCollection<TValue> values)
            => new Group<TKey, TValue>(key, values);
    }

    public class Group<TKey, TValue>
    {
        public Group(TKey key, IReadOnlyCollection<TValue> values)
            => (Key, Values) = (key, values);

        public TKey Key { get; }
        public IReadOnlyCollection<TValue> Values { get; }
    }
}

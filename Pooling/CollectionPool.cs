#nullable enable
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    public class ListPool<T> : CollectionPool<List<T>, T> {}
    public class HashSetPool<T> : CollectionPool<HashSet<T>, T> {}
    public class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> where TKey : notnull {}

    public class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
    {
        private static readonly Pool<TCollection> Pool = new Pool<TCollection>(() => new TCollection(), onObjectRelease: value => value.Clear());

        public static PoolHandle<TCollection> Acquire(out TCollection value)
        {
            return Pool.Acquire(out value);
        }

        public static TCollection Acquire() => Pool.Acquire();

        public static void Release(TCollection value)
        {
            Pool.Release(value);
        }
    }
}

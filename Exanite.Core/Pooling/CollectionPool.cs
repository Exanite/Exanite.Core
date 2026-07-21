using System.Collections.Generic;
using Exanite.Core.Collections;

namespace Exanite.Core.Pooling;

/// <inheritdoc cref="CollectionPool{T, T}"/>>
public abstract class ListPool<T> : CollectionPool<List<T>, T>;

/// <inheritdoc cref="CollectionPool{T, T}"/>>
public abstract class HashSetPool<T> : CollectionPool<HashSet<T>, T>;

/// <inheritdoc cref="CollectionPool{T, T}"/>>
public abstract class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> where TKey : notnull;

/// <summary>
/// A collection pool. Releasing a collection back to the pool will clear it automatically.
/// </summary>
public abstract class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
{
    private static readonly Pool<TCollection> Pool = Create();

    public static Pool<TCollection> Create()
    {
        return new Pool<TCollection>(
            create: () => new TCollection(),
            onRelease: value => value.Clear());
    }

    public static Pool<TCollection>.Handle Acquire(out TCollection value)
    {
        return Pool.Acquire(out value);
    }

    public static TCollection Acquire()
    {
        return Pool.Acquire();
    }

    public static void Release(TCollection value)
    {
        Pool.Release(value);
    }
}

/// <inheritdoc cref="CollectionPool{T, T}"/>>
public class BitSetPool
{
    private static readonly Pool<BitSet> Pool = Create();

    public static Pool<BitSet> Create()
    {
        return new Pool<BitSet>(
            create: () => new BitSet(),
            onRelease: value => value.Clear());
    }

    public static Pool<BitSet>.Handle Acquire(out BitSet value)
    {
        return Pool.Acquire(out value);
    }

    public static BitSet Acquire()
    {
        return Pool.Acquire();
    }

    public static void Release(BitSet value)
    {
        Pool.Release(value);
    }
}

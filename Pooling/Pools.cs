using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Exanite.Core.Pooling;

public static class Pools
{
    // These only need to be sets, but C# does not have concurrent sets:
    // https://github.com/dotnet/runtime/issues/39919
    private static readonly ConcurrentDictionary<Pool, byte> AllPoolsCollection = new();
    private static readonly ConcurrentDictionary<Pool, byte> StaticPoolsCollection = new();

    public static ICollection<Pool> AllPools => AllPoolsCollection.Keys;
    public static ICollection<Pool> StaticPools => StaticPoolsCollection.Keys;

    internal static T AddPool<T>(T pool, bool isStatic) where T : Pool
    {
        AllPoolsCollection.TryAdd(pool, 0);

        if (isStatic)
        {
            StaticPoolsCollection.TryAdd(pool, 0);
        }

        return pool;
    }

    internal static T RemovePool<T>(T pool) where T : Pool
    {
        AllPoolsCollection.TryRemove(pool, out _);
        StaticPoolsCollection.TryRemove(pool, out _);

        return pool;
    }
}

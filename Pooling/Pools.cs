using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    public static class Pools
    {
        private static readonly ConcurrentBag<Pool> StaticPoolsCollection = new();

        public static IReadOnlyCollection<Pool> StaticPools => StaticPoolsCollection;

        internal static T AddStaticPool<T>(T pool) where T : Pool
        {
            StaticPoolsCollection.Add(pool);

            return pool;
        }
    }
}

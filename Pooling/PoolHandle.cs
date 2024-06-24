#nullable enable
using System;

namespace Exanite.Core.Pooling
{
    public struct PoolHandle<T> : IDisposable where T : class
    {
        private readonly T value;
        private readonly ObjectPool<T> pool;

        public PoolHandle(T value, ObjectPool<T> pool)
        {
            this.value = value;
            this.pool = pool;
        }

        public void Dispose()
        {
            pool.Release(value);
        }
    }
}

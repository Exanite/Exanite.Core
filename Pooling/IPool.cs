using System;
using Exanite.Core.Runtime;

namespace Exanite.Core.Pooling
{
    /// <summary>
    /// Defines a minimal interface for pools.
    /// </summary>
    /// <remarks>
    /// This is a convenience interface specialized for <see cref="Pool{T}"/>.
    /// </remarks>
    public interface IPool<T> : IPool<T, Pool<T>.Handle> {}

    /// <summary>
    /// Defines a minimal interface for pools.
    /// </summary>
    /// <remarks>
    /// The use of <see cref="THandle"/> is to prevent boxing of handles, which are usually implemented as structs.
    /// </remarks>
    public interface IPool<TValue, out THandle> where THandle : IDisposable
    {
        public THandle Acquire(out TValue value);
        public TValue Acquire();

        public void Release(TValue element);
    }

    /// <summary>
    /// Mainly used to ensure a consistent interface when wrapping <see cref="Pool{T}"/>.
    /// Users of pools likely want to depend on <see cref="IPool{TValue}"/> or <see cref="IPool{TValue,THandle}"/> instead.
    /// </summary>
    public interface IWrappedPool<T> : IPool<T, Pool<T>.Handle>, ITrackedDisposable
    {
        public PoolUsageInfo UsageInfo { get; }
    }
}

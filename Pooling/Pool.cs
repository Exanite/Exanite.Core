using System;
using System.Collections.Generic;
using System.Threading;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;

namespace Exanite.Core.Pooling;

/// <summary>
/// Allows for the reuse of objects to minimize the allocation of new objects.
/// Objects are acquired from and released back to a shared pool of objects.
/// </summary>
public abstract class Pool : ITrackedDisposable
{
    public bool IsDisposed { get; protected set; }
    public abstract PoolUsageInfo UsageInfo { get; }

    internal Pool() {}

    public abstract void Dispose();
}

/// <inheritdoc cref="Pool"/>
/// <remarks>
/// This class is internally synchronized.
/// </remarks>
public class Pool<T> : Pool, IPool<T>
{
    private readonly Lock sync = new();
    private readonly Stack<T> values = new();

    private readonly Func<T> create;
    private readonly Action<T> onAcquire;
    private readonly Action<T> onRelease;
    private readonly Action<T> onDestroy;

    private PoolUsageInfo usageInfo;

    public override PoolUsageInfo UsageInfo
    {
        get
        {
            lock (sync)
            {
                UpdateUsageInfo();
                return usageInfo;
            }
        }
    }

    public Pool(
        Func<T> create,
        Action<T>? onAcquire = null,
        Action<T>? onRelease = null,
        Action<T>? onDestroy = null,
        int initialMaxInactive = 100,
        bool allowResizing = true)
    {
        if (initialMaxInactive <= 0)
        {
            throw new ArgumentException("initialMaxInactive must be greater than 0", nameof(initialMaxInactive));
        }

        usageInfo.MaxInactive = initialMaxInactive;
        usageInfo.AllowResizing = allowResizing;

        this.create = () =>
        {
            usageInfo.CreateCount++;
            return create.Invoke();
        };

        this.onAcquire = value =>
        {
            usageInfo.AcquireCount++;
            onAcquire?.Invoke(value);
        };

        this.onRelease = value =>
        {
            usageInfo.ReleaseCount++;
            onRelease?.Invoke(value);
        };

        this.onDestroy = value =>
        {
            usageInfo.DestroyCount++;
            onDestroy?.Invoke(value);

            if (!IsDisposed && usageInfo.AllowResizing && usageInfo.DestroyCount > (ulong)usageInfo.MaxInactive)
            {
                usageInfo.MaxInactive *= 2;
                usageInfo.MaxInactiveResizeCount++;
            }
        };
    }

    /// <summary>
    /// Acquire a pooled resource from the pool.
    /// </summary>
    public Handle Acquire(out T value)
    {
        return new Handle(this, value = Acquire());
    }

    /// <summary>
    /// Acquire a pooled resource from the pool.
    /// </summary>
    public T Acquire()
    {
        lock (sync)
        {
            AssertUtility.IsFalse(IsDisposed, "Pool has been disposed");

            if (!values.TryPop(out var value))
            {
                value = create.Invoke();
                usageInfo.TotalCount++;
            }

            onAcquire.Invoke(value);

            return value;
        }
    }

    /// <summary>
    /// Release a pooled resource back to the pool to be reused later.
    /// </summary>
    /// <remarks>
    /// Calling this method after the pool has been disposed is allowed.
    /// In this case, the resource will be immediately disposed.
    /// </remarks>
    public void Release(T value)
    {
        lock (sync)
        {
            onRelease.Invoke(value);

            UpdateUsageInfo();
            if (!IsDisposed && usageInfo.InactiveCount < usageInfo.MaxInactive)
            {
                values.Push(value);
            }
            else
            {
                usageInfo.TotalCount--;
                onDestroy.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Disposes all objects in the pool and clears the pool.
    /// </summary>
    public void Clear()
    {
        lock (sync)
        {
            foreach (var value in values)
            {
                usageInfo.TotalCount--;
                onDestroy.Invoke(value);
            }

            values.Clear();
        }
    }

    private void UpdateUsageInfo()
    {
        usageInfo.InactiveCount = values.Count;
        usageInfo.ActiveCount = usageInfo.TotalCount - usageInfo.InactiveCount;
    }

    public override void Dispose()
    {
        lock (sync)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            Clear();
        }
    }

    public readonly struct Handle : IDisposable
    {
        public readonly Pool<T> Pool;
        public readonly T Value;

        public Handle(Pool<T> pool, T value)
        {
            Pool = pool;
            Value = value;
        }

        public void Dispose()
        {
            Pool.Release(Value);
        }
    }
}

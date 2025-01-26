using System;
using System.Collections.Generic;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;

namespace Exanite.Core.Pooling
{
    public abstract class Pool : ITrackedDisposable
    {
        public bool IsDisposed { get; protected set; }
        public abstract PoolUsageInfo UsageInfo { get; }

        public abstract void Dispose();
    }

    /// <summary>
    /// Conventional object pool where objects can be acquired and released.
    /// </summary>
    public class Pool<T> : Pool, IPool<T>
    {
        private readonly Queue<T> values;

        private readonly Func<T> create;
        private readonly Action<T> onAcquire;
        private readonly Action<T> onRelease;
        private readonly Action<T> onDestroy;

        private PoolUsageInfo usageInfo;

        public override PoolUsageInfo UsageInfo
        {
            get
            {
                UpdateUsageInfo();
                return usageInfo;
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

            values = new Queue<T>();
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

            Pools.AddPool(this, false);
        }

        public Handle Acquire(out T value)
        {
            return new Handle(this, value = Acquire());
        }

        public T Acquire()
        {
            AssertUtility.IsFalse(IsDisposed, "Pool has been disposed");

            if (values.Count == 0)
            {
                values.Enqueue(create());
                usageInfo.TotalCount++;
            }

            var value = values.Dequeue();
            onAcquire.Invoke(value);

            return value;
        }

        public void Release(T element)
        {
            onRelease.Invoke(element);

            UpdateUsageInfo();
            if (!IsDisposed && usageInfo.InactiveCount < usageInfo.MaxInactive)
            {
                values.Enqueue(element);
            }
            else
            {
                usageInfo.TotalCount--;
                onDestroy.Invoke(element);
            }
        }

        public void Clear()
        {
            foreach (var value in values)
            {
                usageInfo.TotalCount--;
                onDestroy.Invoke(value);
            }

            values.Clear();
        }

        private void UpdateUsageInfo()
        {
            usageInfo.InactiveCount = values.Count;
            usageInfo.ActiveCount = usageInfo.TotalCount - usageInfo.InactiveCount;
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            Clear();
            Pools.RemovePool(this);
        }

        public struct Handle : IDisposable
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
}

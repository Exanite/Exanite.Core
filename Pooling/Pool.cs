using System;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    public abstract class Pool : IDisposable
    {
        public abstract void Dispose();
    }

    /// <summary>
    /// Conventional object pool where objects can be acquired and released.
    /// </summary>
    public class Pool<T> : Pool
    {
        private readonly Queue<T> values;

        private readonly Func<T> create;
        private readonly Action<T>? onAcquire;
        private readonly Action<T>? onRelease;
        private readonly Action<T>? onDestroy;

        public int MaxInactive { get; private set; }

        public int TotalCount { get; private set; }
        public int ActiveCount => TotalCount - InactiveCount;
        public int InactiveCount => values.Count;

        public Pool(
            Func<T> create,
            Action<T>? onAcquire = null,
            Action<T>? onRelease = null,
            Action<T>? onDestroy = null,
            int maxInactive = 100)
        {
            if (maxInactive <= 0)
            {
                throw new ArgumentException("Max inactive must be greater than 0", nameof(maxInactive));
            }

            values = new Queue<T>();
            MaxInactive = maxInactive;

            this.create = create;
            this.onAcquire = onAcquire;
            this.onRelease = onRelease;
            this.onDestroy = onDestroy;
        }

        public Handle Acquire(out T value)
        {
            return new Handle(this, value = Acquire());
        }

        public T Acquire()
        {
            if (values.Count == 0)
            {
                values.Enqueue(create());
                TotalCount++;
            }

            var value = values.Dequeue();

            onAcquire?.Invoke(value);

            return value;
        }

        public void Release(T element)
        {
            var actionOnRelease = onRelease;
            if (actionOnRelease != null)
            {
                actionOnRelease(element);
            }

            if (InactiveCount < MaxInactive)
            {
                values.Enqueue(element);
            }
            else
            {
                TotalCount--;
                onDestroy?.Invoke(element);
            }
        }

        public void Clear()
        {
            if (onDestroy != null)
            {
                foreach (var value in values)
                {
                    onDestroy(value);
                }
            }

            values.Clear();
        }

        public override void Dispose()
        {
            Clear();
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

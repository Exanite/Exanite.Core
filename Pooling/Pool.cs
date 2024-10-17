using System;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    /// <summary>
    /// Conventional object pool where objects can be acquired and released.
    /// </summary>
    public class Pool<T> : IDisposable where T : class
    {
        private readonly Queue<T> values;

        private readonly Func<T> create;
        private readonly Action<T>? onGet;
        private readonly Action<T>? onRelease;
        private readonly Action<T>? onDestroy;

        public int MaxInactive { get; private set; }

        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => values.Count;

        public Pool(
            Func<T> create,
            Action<T>? onGet = null,
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
            this.onGet = onGet;
            this.onRelease = onRelease;
            this.onDestroy = onDestroy;
        }

        public PoolHandle<T> Acquire(out T value)
        {
            return new PoolHandle<T>(value = Acquire(), this);
        }

        public T Acquire()
        {
            if (values.Count == 0)
            {
                values.Enqueue(create());
                CountAll++;
            }

            var value = values.Dequeue();

            onGet?.Invoke(value);

            return value;
        }

        public void Release(T element)
        {
            var actionOnRelease = onRelease;
            if (actionOnRelease != null)
            {
                actionOnRelease(element);
            }

            if (CountInactive < MaxInactive)
            {
                values.Enqueue(element);
            }
            else
            {
                CountAll--;
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

        public void Dispose()
        {
            Clear();
        }
    }
}

using System;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    /// <summary>
    /// Conventional object pool where objects can be acquired and released.
    /// </summary>
    public class Pool<T> : IDisposable where T : class
    {
        private readonly List<T> values;

        private readonly Func<T> create;
        private readonly Action<T>? onGet;
        private readonly Action<T>? onRelease;
        private readonly Action<T>? onDestroy;

        public int MaxCapacity { get; private set; }

        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => values.Count;

        public Pool(
            Func<T> create,
            Action<T>? onGet = null,
            Action<T>? onRelease = null,
            Action<T>? onDestroy = null,
            int defaultCapacity = 10,
            int maxCapacity = 10000)
        {
            if (defaultCapacity <= 0)
            {
                throw new ArgumentException("Default capacity must be greater than 0", nameof(defaultCapacity));
            }

            if (maxCapacity <= 0)
            {
                throw new ArgumentException("Max capacity must be greater than 0", nameof(maxCapacity));
            }

            values = new List<T>(defaultCapacity);
            MaxCapacity = maxCapacity;

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
                values.Add(create());
                CountAll++;
            }

            var index = values.Count - 1;
            var value = values[index];
            values.RemoveAt(index);

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

            if (CountInactive < MaxCapacity)
            {
                values.Add(element);
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

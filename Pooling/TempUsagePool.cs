#nullable enable
using System;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    /// <summary>
    /// Object pool where the pooled objects are expected to be used for a short duration and batch released by the owner of the pool.
    /// </summary>
    /// <example>
    /// Objects can be acquired throughout a frame and batch released at the end of the frame.
    /// </example>
    public class TempUsagePool<T> where T : class
    {
        private readonly List<T> values;

        private readonly Func<T> create;
        private readonly Action<T>? onGet;
        private readonly Action<T>? onRelease;
        private readonly Action<T>? onDestroy;

        public int TotalCount => values.Count;
        public int AcquiredCount { get; private set; }
        public int MaxCapacity { get; private set; }

        public TempUsagePool(
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

        public T Acquire()
        {
            if (AcquiredCount >= values.Count)
            {
                values.Add(create());
            }

            var value = values[AcquiredCount];
            AcquiredCount++;

            onGet?.Invoke(value);

            return value;
        }

        public void ReleaseAll()
        {
            for (var i = 0; i < AcquiredCount; i++)
            {
                onRelease?.Invoke(values[i]);
            }

            AcquiredCount = 0;
        }

        public void Clear()
        {
            ReleaseAll();

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

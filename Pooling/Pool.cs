#nullable enable
using System;
using System.Collections.Generic;

namespace Exanite.Core.Pooling
{
    public class Pool<T> : IDisposable where T : class
    {
        private readonly List<T> values;

        private readonly Func<T> createObject;
        private readonly Action<T>? onObjectGet;
        private readonly Action<T>? onObjectRelease;
        private readonly Action<T>? onObjectDestroy;

        public int MaxCapacity { get; private set; }

        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => values.Count;

        public Pool(
            Func<T> createObject,
            Action<T>? onObjectGet = null,
            Action<T>? onObjectRelease = null,
            Action<T>? onObjectDestroy = null,
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

            this.createObject = createObject;
            this.onObjectGet = onObjectGet;
            this.onObjectRelease = onObjectRelease;
            this.onObjectDestroy = onObjectDestroy;
        }

        public PoolHandle<T> Acquire(out T value)
        {
            return new PoolHandle<T>(value = Acquire(), this);
        }

        public T Acquire()
        {
            if (values.Count == 0)
            {
                values.Add(createObject());
                CountAll++;
            }

            var index = values.Count - 1;
            var value = values[index];
            values.RemoveAt(index);

            onObjectGet?.Invoke(value);

            return value;
        }

        public void Release(T element)
        {
            var actionOnRelease = onObjectRelease;
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
                onObjectDestroy?.Invoke(element);
            }
        }

        public void Clear()
        {
            if (onObjectDestroy != null)
            {
                foreach (var value in values)
                {
                    onObjectDestroy(value);
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

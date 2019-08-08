using System;
using System.Collections.Generic;

namespace Exanite.Core.ObjectPooling
{
    public class ObjectPool<T>
    {
        /// <summary>
        /// Current size of the pool
        /// </summary>
        public int CurrentSize => Pool.Count;

        private Queue<T> Pool { get; } = new Queue<T>();

        private Func<T> Factory { get; }

        /// <summary>
        /// Creates a new <see cref="ObjectPool{T}"/> with an initial size of 10
        /// </summary>
        public ObjectPool() : this(10, null) { }

        /// <summary>
        /// Creates a new <see cref="ObjectPool{T}"/>
        /// </summary>
        public ObjectPool(int initialSize, Func<T> factory = null)
        {
            Factory = factory ?? Activator.CreateInstance<T>;

            Expand(initialSize);
        }

        /// <summary>
        /// Expands the pool by an amount
        /// </summary>
        public virtual void Expand(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Pool.Enqueue(Factory());
            }
        }

        /// <summary>
        /// Acquire an object from the pool
        /// </summary>
        public virtual T Acquire()
        {
            T result;
            if (Pool.Count > 0)
            {
                result = Pool.Dequeue();
            }
            else
            {
                result = Factory();
            }

            if (result is IPoolable poolable)
            {
                poolable.OnAcquired();
            }

            return result;
        }

        /// <summary>
        /// Release an object back to the pool
        /// </summary>
        public virtual void Release(T obj)
        {
            if (obj is IPoolable poolable)
            {
                poolable.OnReleased();
            }

            Pool.Enqueue(obj);
        }
    }
}

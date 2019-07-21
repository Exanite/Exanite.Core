using System;
using System.Collections.Generic;

namespace Exanite.Core.ObjectPooling
{
    public class ObjectPool<T>
    {
        public int CurrentSize => Pool.Count;

        private Queue<T> Pool { get; } = new Queue<T>();
        private Func<T> Factory { get; }

        public ObjectPool() : this(10, null) { }

        public ObjectPool(int initialSize, Func<T> factory = null)
        {
            Factory = factory ?? Activator.CreateInstance<T>;

            Expand(initialSize);
        }

        public virtual void Expand(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Pool.Enqueue(Factory());
            }
        }

        public virtual T Get()
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
                poolable.OnGet();
            }

            return result;
        }

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

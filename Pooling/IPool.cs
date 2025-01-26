using Exanite.Core.Runtime;

namespace Exanite.Core.Pooling
{
    public interface IPool<T> : ITrackedDisposable
    {
        public PoolUsageInfo UsageInfo { get; }
        
        public Pool<T>.Handle Acquire(out T value);
        public T Acquire();
        
        public void Release(T element);
        
        public void Clear();
    }
}

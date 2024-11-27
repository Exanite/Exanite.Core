using System.Text;

namespace Exanite.Core.Pooling
{
    public class StringBuilderPool
    {
        private static readonly Pool<StringBuilder> Pool = Pools.AddStaticPool(Create());

        public static Pool<StringBuilder> Create()
        {
            return new Pool<StringBuilder>(
                create: () => new StringBuilder(),
                onRelease: value => value.Clear());
        }

        public static Pool<StringBuilder>.Handle Acquire(out StringBuilder value)
        {
            lock (Pool)
            {
                return Pool.Acquire(out value);
            }
        }

        public static StringBuilder Acquire()
        {
            lock (Pool)
            {
                return Pool.Acquire();
            }
        }

        public static void Release(StringBuilder value)
        {
            lock (Pool)
            {
                Pool.Release(value);
            }
        }
    }
}

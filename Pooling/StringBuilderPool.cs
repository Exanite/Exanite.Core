using System.Text;

namespace Exanite.Core.Pooling;

/// <summary>
/// A <see cref="StringBuilder"/> pool. Releasing a <see cref="StringBuilder"/> back to the pool will clear it automatically.
/// </summary>
public abstract class StringBuilderPool
{
    private static readonly Pool<StringBuilder> Pool = Pools.AddPool(Create(), true);

    public static Pool<StringBuilder> Create()
    {
        return new Pool<StringBuilder>(
            create: () => new StringBuilder(),
            onRelease: value => value.Clear());
    }

    public static Pool<StringBuilder>.Handle Acquire(out StringBuilder value)
    {
        return Pool.Acquire(out value);
    }

    public static StringBuilder Acquire()
    {
        return Pool.Acquire();
    }

    public static void Release(StringBuilder value)
    {
        Pool.Release(value);
    }
}

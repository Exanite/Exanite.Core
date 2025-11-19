namespace Exanite.Core.Pooling;

/// <inheritdoc cref="SimplePool{T}"/>>
public abstract class SimplePool
{
    public static void Release<T>(T value) where T : new()
    {
        SimplePool<T>.Release(value);
    }
}

/// <summary>
/// Pools trivially constructible objects.
/// <br/>
/// Warning: Does not handle cleanup such as object disposal or clearing.
/// </summary>
public abstract class SimplePool<T> where T : new()
{
    private static readonly Pool<T> Pool = Pools.AddPool(Create(), true);

    public static Pool<T> Create()
    {
        return new Pool<T>(create: () => new T());
    }

    public static Pool<T>.Handle Acquire(out T value)
    {
        return Pool.Acquire(out value);
    }

    public static T Acquire()
    {
        return Pool.Acquire();
    }

    public static void Release(T value)
    {
        Pool.Release(value);
    }
}

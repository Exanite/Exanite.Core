namespace Exanite.Core.Runtime;

/// <summary>
/// Utility methods for <see cref="DoubleBuffer{T}"/>.
/// </summary>
public static class DoubleBuffer
{
    public static DoubleBuffer<T> Create<T>() where T : new()
    {
        return new DoubleBuffer<T>(new T(), new T());
    }
}

/// <summary>
/// Simple double buffer implementation.
/// </summary>
public class DoubleBuffer<T>
{
    public T Read { get; private set; }
    public T Write { get; private set; }

    public DoubleBuffer(T read, T write)
    {
        Read = read;
        Write = write;
    }

    /// <summary>
    /// Swap the read and write resources.
    /// </summary>
    /// <returns>The read resource.</returns>
    public T Swap()
    {
        (Read, Write) = (Write, Read);

        return Read;
    }
}

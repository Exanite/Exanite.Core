using System;

namespace Exanite.Core.Runtime;

/// <summary>
/// Utility methods for <see cref="ValueDoubleBuffer{T}"/>.
/// </summary>
public static class ValueDoubleBuffer
{
    public static ValueDoubleBuffer<T> Create<T>() where T : new()
    {
        return new ValueDoubleBuffer<T>(new T(), new T());
    }
}

/// <summary>
/// Simple double buffer implementation as a struct.
/// </summary>
/// <remarks>
/// See <see cref="DoubleBuffer"/> for a class version.
/// </remarks>
public struct ValueDoubleBuffer<T>
{
    /// <summary>
    /// The read resource. Used by the consumer.
    /// </summary>
    public T Read { get; private set; }

    /// <summary>
    /// The write resource. Used by the producer.
    /// </summary>
    public T Write { get; private set; }

    /// <summary>
    /// Create a double buffer with the provided resources.
    /// </summary>
    /// <param name="read">The resource that will be first used as the read resource.</param>
    /// <param name="write">The resource that will be first used as the write resource.</param>
    public ValueDoubleBuffer(T read, T write)
    {
        Read = read;
        Write = write;
    }

    /// <summary>
    /// Create a double buffer using the resources created by the function to create the read and write resources.
    /// </summary>
    /// <param name="create">Create the resource. The int parameter is the number of resources created so far.</param>
    public ValueDoubleBuffer(Func<int, T> create)
    {
        Read = create.Invoke(0);
        Write = create.Invoke(1);
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

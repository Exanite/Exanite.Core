namespace Exanite.Core.Runtime;

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
    public void Swap()
    {
        (Read, Write) = (Write, Read);
    }
}

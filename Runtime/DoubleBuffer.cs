namespace Exanite.Core.Runtime;

/// <summary>
/// Simple double buffer implementation.
/// </summary>
public struct DoubleBuffer<T>
{
    public T Read { get; private set; }
    public T Write { get; private set; }

    /// <summary>
    /// Swap the read and write resources.
    /// </summary>
    public void Swap()
    {
        (Read, Write) = (Write, Read);
    }
}

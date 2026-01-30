namespace Exanite.Core.Runtime;

public abstract class Box
{
    internal Box() {}
}

/// <summary>
/// Wraps a value inside a heap allocated class.
/// This is mainly intended to be used as a way to avoid repeated GC allocations when storing value types as objects.
/// </summary>
public class Box<T> : Box
{
    public T Value;

    public Box(T value)
    {
        Value = value;
    }
}

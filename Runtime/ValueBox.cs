namespace Exanite.Core.Runtime;

public abstract class ValueBox
{
    internal ValueBox() {}
}

/// <summary>
/// Wraps a value inside a heap allocated class.
/// This is mainly intended to be used as a way to avoid repeated GC allocations when storing value types as objects.
/// </summary>
public class ValueBox<T> : ValueBox
{
    public T Value;

    public ValueBox(T value)
    {
        Value = value;
    }
}

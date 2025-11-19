namespace Exanite.Core.Runtime;

public abstract class VBox
{
    internal VBox() {}
}

/// <summary>
/// Wraps a value inside a heap allocated class.
/// This is mainly intended to be used as a way to avoid repeated GC allocations when storing value types as objects.
/// </summary>
public class VBox<T> : VBox
{
    public T Value;

    public VBox(T value)
    {
        Value = value;
    }
}

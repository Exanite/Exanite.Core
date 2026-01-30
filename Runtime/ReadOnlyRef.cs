namespace Exanite.Core.Runtime;

/// <summary>
/// Wraps a readonly ref to a value inside a ref struct.
/// This can be useful for ensuring that refs do not get accidentally dropped or for returning refs from TryGet methods.
/// </summary>
/// <remarks>
/// See <see cref="Ref{T}"/> for a mutable version.
/// Originally called a ReadOnlyValueRef, but shortened since this is a commonly used type.
/// </remarks>
public readonly ref struct ReadOnlyRef<T>
{
    public readonly ref readonly T Value;

    public ReadOnlyRef(ref T value)
    {
        Value = ref value;
    }
}

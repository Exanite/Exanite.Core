namespace Exanite.Core.Runtime;

/// <summary>
/// Wraps a readonly ref to a value inside a ref struct.
/// This can be useful for ensuring that refs do not get accidentally dropped or for returning refs from TryGet methods.
/// </summary>
/// <remarks>
/// See <see cref="VRef{T}"/> for a mutable version.
/// </remarks>
public readonly ref struct ReadOnlyVRef<T>
{
    public readonly ref readonly T Value;

    public ReadOnlyVRef(ref T value)
    {
        Value = ref value;
    }
}

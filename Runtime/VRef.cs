namespace Exanite.Core.Runtime;

/// <summary>
/// Wraps a ref to a value inside a ref struct.
/// This can be useful for ensuring that refs do not get accidentally dropped or for returning refs from TryGet methods.
/// </summary>
/// <remarks>
/// This is not named Ref because of Silk.Net 3's Ref struct.
/// See <see cref="ReadOnlyVRef{T}"/> for a readonly version.
/// </remarks>
public readonly ref struct VRef<T>
{
    public readonly ref T Value;

    public VRef(ref T value)
    {
        Value = ref value;
    }
}

namespace Exanite.Core.Runtime;

/// <summary>
/// Wraps a ref to a value inside a ref struct.
/// This can be useful for ensuring that refs do not get accidentally dropped or for returning refs from TryGet methods.
/// </summary>
/// <remarks>
/// See <see cref="ReadOnlyRef{T}"/> for a readonly version.
/// </remarks>
public readonly ref struct Ref<T>
{
    public readonly ref T Value;

    public Ref(ref T value)
    {
        Value = ref value;
    }
}

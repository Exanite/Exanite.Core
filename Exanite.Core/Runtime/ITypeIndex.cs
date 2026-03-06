namespace Exanite.Core.Runtime;

/// <summary>
/// Can be used to acquire sequential indexes unique to each input type.
/// The first index starts at 0 and grows sequentially for new types accessed.
/// </summary>
public interface ITypeIndex
{
    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// The first index starts at 0 and grows sequentially for new types accessed.
    /// </summary>
    public static abstract int Get<T>() where T : allows ref struct;
}

using System.Runtime.CompilerServices;
using System.Threading;

namespace Exanite.Core.Runtime;

/// <summary>
/// Can be used to acquire sequential indexes unique to each input type.
/// The first index starts at 0 and grows sequentially for new types accessed.
/// </summary>
/// <typeparam name="TScope">
/// The type used as the scope for the generated indices.
/// Because the indices are global and often used to index lists (see <see cref="TypeIndexedList{TScope, TValue}"/>),
/// this can be used to avoid polluting the indices with unrelated types.
/// </typeparam>
public abstract class TypeIndex<TScope>
{
    private TypeIndex() {}

    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// The first index starts at 0 and grows sequentially for new types accessed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get<T>() where T : allows ref struct
    {
        return TypeIndex<TScope, T>.Value;
    }

    /// <summary>
    /// 0 is the first valid index.
    /// </summary>
    private static int PreviousIndex = -1;
    internal static int GetNext() => Interlocked.Increment(ref PreviousIndex);
}

/// <summary>
/// Assigns a unique index to each type.
/// </summary>
internal static class TypeIndex<TScope, T> where T : allows ref struct
{
    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This property is cached, making repeated accesses very efficient.
    /// </remarks>
    public static readonly int Value = TypeIndex<TScope>.GetNext();
}

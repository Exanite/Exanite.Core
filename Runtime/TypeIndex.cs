using System.Runtime.CompilerServices;
using System.Threading;

namespace Exanite.Core.Runtime;

public abstract class TypeIndex<TScope>
{
    private TypeIndex() {}

    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get<T>() where T : allows ref struct
    {
        return TypeIndex<TScope, T>.Value;
    }

    /// <summary>
    /// 1 is the first valid index.
    /// </summary>
    private static int PreviousIndex = 0;
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

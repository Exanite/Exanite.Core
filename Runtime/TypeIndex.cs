using System.Runtime.CompilerServices;
using System.Threading;

namespace Exanite.Core.Runtime;

public static class TypeIndex
{
    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get<T>() where T : allows ref struct
    {
        return TypeIndex<T>.Value;
    }

    private static int PreviousId = -1;
    internal static int GetNext() => Interlocked.Increment(ref PreviousId);
}

/// <summary>
/// Assigns a unique index to each type.
/// </summary>
internal static class TypeIndex<T> where T : allows ref struct
{
    /// <summary>
    /// The unique index for <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This property is cached, making repeated accesses very efficient.
    /// </remarks>
    public static readonly int Value = TypeIndex.GetNext();
}

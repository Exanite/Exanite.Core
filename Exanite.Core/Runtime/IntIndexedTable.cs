using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// A table indexed by an integer index.
/// </summary>
/// <remarks>
/// The backing storage is a list that grows to accommodate the highest accessed index.
/// In most usage cases, this backing list will be sparse.
/// </remarks>
public class IntIndexedTable<TValue>
{
    private readonly List<TValue?> values = new();

    /// <summary>
    /// Returns whether this list has a slot for the specified index.
    /// This does not check whether the value stored in the slot is null or default.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInBounds(int index)
    {
        return (uint)index < (uint)values.Count;
    }

    /// <summary>
    /// Returns the stored value at the specified index.
    /// </summary>
    /// <remarks>
    /// This can resize the internal list.
    /// Be very careful with what values you pass to this method
    /// because the list will grow so that it contains the specified index.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TValue? Get(int index)
    {
        CollectionsMarshal.SetCount(values, int.Max(values.Count, index + 1));
        return ref values.AsSpan()[index];
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
        values.Clear();
    }
}

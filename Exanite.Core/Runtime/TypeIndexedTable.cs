using System.Collections.Generic;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// A table indexed by the type.
/// Can be used as a high performance replacement for <see cref="Dictionary{Type, TValue}"/>
/// in cases where all accesses are strongly typed.
/// </summary>
/// <remarks>
/// The backing storage is a list that grows to accommodate the highest accessed index.
/// In most usage cases, this backing list will be sparse.
/// </remarks>
public class TypeIndexedTable<TTypeIndex, TValue> where TTypeIndex : ITypeIndex
{
    private readonly List<TValue?> values = new();

    /// <summary>
    /// Returns whether this list has a slot for the specified type.
    /// This does not check whether the value stored in the slot is null or default.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    public bool IsInBounds<TType>() where TType : allows ref struct
    {
        var typeIndex = GetIndex<TType>();
        return (uint)typeIndex < (uint)values.Count;
    }

    /// <summary>
    /// Returns whether this list has a slot for the specified index.
    /// This does not check whether the value stored in the slot is null or default.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    public bool IsInBounds(int index)
    {
        return (uint)index < (uint)values.Count;
    }

    /// <summary>
    /// Returns the stored value at the specified index.
    /// </summary>
    /// <remarks>
    /// This can resize the internal list.
    /// </remarks>
    public ref TValue? Get(int index)
    {
        CollectionsMarshal.SetCount(values, int.Max(values.Count, index + 1));
        return ref values.AsSpan()[index];
    }

    /// <summary>
    /// Returns the stored value for the specified type.
    /// </summary>
    /// <remarks>
    /// This can resize the internal list.
    /// </remarks>
    public ref TValue? Get<TType>() where TType : allows ref struct
    {
        var typeIndex = GetIndex<TType>();
        CollectionsMarshal.SetCount(values, int.Max(values.Count, typeIndex + 1));
        return ref values.AsSpan()[typeIndex];
    }

    /// <summary>
    /// Gets the corresponding index for the specified type.
    /// This index can be out of bounds.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    public int GetIndex<TType>() where TType : allows ref struct
    {
        return TTypeIndex.Get<TType>();
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
        values.Clear();
    }
}

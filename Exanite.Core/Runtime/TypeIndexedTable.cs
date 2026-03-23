using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
public class TypeIndexedTable<TTypeIndex, TValue> : IndexedTable<TValue> where TTypeIndex : ITypeIndex
{
    /// <summary>
    /// Returns whether this list has a slot for the specified type.
    /// This does not check whether the value stored in the slot is null or default.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInBounds<TType>() where TType : allows ref struct
    {
        return IsInBounds(GetIndex<TType>());
    }

    /// <summary>
    /// Returns the stored value for the specified type.
    /// </summary>
    /// <remarks>
    /// This can resize the internal list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TValue? Get<TType>() where TType : allows ref struct
    {
        return ref Get(GetIndex<TType>());
    }

    /// <summary>
    /// Gets the corresponding index for the specified type.
    /// This index can be out of bounds.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex<TType>() where TType : allows ref struct
    {
        return TTypeIndex.Get<TType>();
    }
}

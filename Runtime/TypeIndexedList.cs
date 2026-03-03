using System.Collections.Generic;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// A list indexed by the type.
/// Can be used as a high performance replacement for <see cref="Dictionary{Type, TValue}"/>
/// in cases where all accesses are strongly typed.
/// </summary>
/// <remarks>
/// The backing storage is a list that grows to accommodate the highest stored index.
/// In most usage cases, this backing list will be sparse.
/// </remarks>
/// <typeparam name="TScope">See <see cref="TypeIndex{TScope}"/> for information about <see cref="TScope"/>.</typeparam>
/// <typeparam name="TValue">The type of stored values.</typeparam>
public class TypeIndexedList<TScope, TValue>
{
    private readonly List<TValue?> valuesByTypeIndex = new();

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
        return (uint)typeIndex < (uint)valuesByTypeIndex.Count;
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
        return (uint)index < (uint)valuesByTypeIndex.Count;
    }

    /// <summary>
    /// Returns the stored value at the specified index.
    /// </summary>
    /// <remarks>
    /// This will not resize the internal list.
    /// </remarks>
    public ref TValue? Get(int index)
    {
        return ref valuesByTypeIndex.AsSpan()[index];
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
        CollectionsMarshal.SetCount(valuesByTypeIndex, int.Max(valuesByTypeIndex.Count, typeIndex + 1));

        return ref valuesByTypeIndex.AsSpan()[typeIndex];
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
        return TypeIndex<TScope>.Get<TType>();
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
        valuesByTypeIndex.Clear();
    }
}

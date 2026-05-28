using System;
using System.Collections.Generic;

namespace Exanite.Core.Collections;

public interface IReadOnlyBitSet : IEnumerable<int>
{
    /// <inheritdoc cref="BitSet.Chunks"/>
    public ReadOnlySpan<ulong> Chunks { get; }

    /// <inheritdoc cref="BitSet.Count"/>
    public int Count { get; }

    /// <inheritdoc cref="BitSet.IsEmpty"/>
    public bool IsEmpty { get; }

    /// <inheritdoc cref="BitSet.this"/>
    public bool this[int bitIndex] { get; }

    public void UnionWith(IReadOnlyBitSet other, BitSet results);
    public void IntersectWith(IReadOnlyBitSet other, BitSet results);
    public void ExceptWith(IReadOnlyBitSet other, BitSet results);
    public void SymmetricExceptWith(IReadOnlyBitSet other, BitSet results);

    public bool IsProperSubsetOf(IReadOnlyBitSet other);
    public bool IsProperSupersetOf(IReadOnlyBitSet other);
    public bool IsSubsetOf(IReadOnlyBitSet other);
    public bool IsSupersetOf(IReadOnlyBitSet other);
    public bool Overlaps(IReadOnlyBitSet other);
    public bool SetEquals(IReadOnlyBitSet other);

    /// <inheritdoc cref="BitSet.this"/>
    public void CopyTo(BitSet other);

    public new BitSetEnumerator GetEnumerator();
}

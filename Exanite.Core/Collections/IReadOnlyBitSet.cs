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

    public BitSet UnionWith(IReadOnlyBitSet other, BitSet results);
    public BitSet IntersectWith(IReadOnlyBitSet other, BitSet results);
    public BitSet ExceptWith(IReadOnlyBitSet other, BitSet results);
    public BitSet SymmetricExceptWith(IReadOnlyBitSet other, BitSet results);

    public bool IsProperSubsetOf(IReadOnlyBitSet other);
    public bool IsProperSupersetOf(IReadOnlyBitSet other);
    public bool IsSubsetOf(IReadOnlyBitSet other);
    public bool IsSupersetOf(IReadOnlyBitSet other);
    public bool Overlaps(IReadOnlyBitSet other);
    public bool SetEquals(IReadOnlyBitSet other);

    /// <inheritdoc cref="BitSet.CopyTo"/>
    public void CopyTo(BitSet other);

    public new BitSetEnumerator GetEnumerator();
}

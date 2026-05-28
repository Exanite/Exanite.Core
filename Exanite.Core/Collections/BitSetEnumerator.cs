using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Exanite.Core.Collections;

public struct BitSetEnumerator : IEnumerator<int>
{
    private readonly IReadOnlyBitSet bitset;

    private int bitI = -1;

    public int Current { get; private set; } = -1;
    object IEnumerator.Current => Current;

    public BitSetEnumerator(IReadOnlyBitSet bitset)
    {
        this.bitset = bitset;
    }

    public bool MoveNext()
    {
        bitI++;

        var chunkI = bitI >> BitSet.Shift;
        var bitInChunkI = bitI & BitSet.Mask;
        while (chunkI < bitset.Chunks.Length)
        {
            var chunk = bitset.Chunks[chunkI];

            // Ignore already visited bits
            chunk >>= bitInChunkI;

            if (chunk != 0)
            {
                // Find next 1 bit
                var trailingZeroCount = BitOperations.TrailingZeroCount(chunk);
                bitI += trailingZeroCount + 1;
                Current = bitI - 1;

                return true;
            }

            // Chunk has no more 1 bits
            bitI += BitSet.BitsPerChunk - bitInChunkI;
            chunkI = bitI >> BitSet.Shift;
            bitInChunkI = bitI & BitSet.Mask;
        }

        return false;
    }

    public void Reset() => throw new NotSupportedException();
    public void Dispose() {}
}

using System;

namespace Exanite.Core.Collections;

public class BitSet
{
    private const int DefaultChunkCount = 4;
    private const int GrowFactor = 2;
    private const int BitsPerChunk = sizeof(int) * 8;
    private const int Shift = 5;
    private const int Mask = (1 << 5) - 1;

    private int[] chunks = new int[DefaultChunkCount];

    public int ChunkCount => Chunks.Length;
    public int BitCount => ChunkCount * BitsPerChunk;

    public ReadOnlySpan<int> Chunks => chunks;

    /// <summary>
    /// Gets or sets the bit at the specified index.
    /// </summary>
    public bool this[int bitIndex]
    {
        get
        {
            var chunkIndex = bitIndex >> Shift;
            if (chunks.Length <= chunkIndex)
            {
                return false;
            }

            var bit = bitIndex & Mask;
            return (chunks[chunkIndex] & (1 << bit)) != 0;
        }
        set
        {
            var chunkIndex = bitIndex >> Shift;
            EnsureCapacity(chunkIndex);

            var bit = bitIndex & Mask;
            if (value)
            {
                chunks[chunkIndex] |= 1 << bit;
            }
            else
            {
                chunks[chunkIndex] &= ~(1 << bit);
            }
        }
    }

    private void EnsureCapacity(int chunkCount)
    {
        if (ChunkCount >= chunkCount)
        {
            return;
        }

        var newChunkCount = ChunkCount;
        while (newChunkCount < chunkCount)
        {
            newChunkCount *= GrowFactor;
        }

        var newChunks = new int[newChunkCount];
        chunks.CopyTo(newChunks);
        chunks = newChunks;
    }
}

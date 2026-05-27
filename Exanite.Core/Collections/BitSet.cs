using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Exanite.Core.Collections;

public class BitSet
{
    private const int DefaultChunkCount = 4;
    private const int GrowFactor = 2;
    private const int BitsPerChunk = sizeof(int) * 8;
    private const int Shift = 5;
    private const int Mask = (1 << 5) - 1;

    private int[] chunks = new int[DefaultChunkCount];

    /// <summary>
    /// The underlying memory used to store the bits.
    /// </summary>
    public ReadOnlySpan<int> Chunks => chunks;

    /// <summary>
    /// Returns whether the bitset contains all false bits.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            var span = Chunks;

            if (Vector256.IsHardwareAccelerated && span.Length >= Vector256<int>.Count)
            {
                var vectorSpan = MemoryMarshal.Cast<int, Vector256<int>>(span);
                foreach (var vector in vectorSpan)
                {
                    if (vector != Vector256<int>.Zero)
                    {
                        return false;
                    }
                }

                span = span[(vectorSpan.Length * Vector256<int>.Count)..];
            }
            else if (Vector128.IsHardwareAccelerated && span.Length >= Vector128<int>.Count)
            {
                var vectorSpan = MemoryMarshal.Cast<int, Vector128<int>>(span);
                foreach (var vector in vectorSpan)
                {
                    if (vector != Vector128<int>.Zero)
                    {
                        return false;
                    }
                }

                span = span[(vectorSpan.Length * Vector128<int>.Count)..];
            }

            foreach (var chunk in span)
            {
                if (chunk != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

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

    /// <summary>
    /// Clears the bitset.
    /// Does not resize the underlying array.
    /// </summary>
    public void Clear()
    {
        Array.Clear(chunks);
    }

    private void EnsureCapacity(int requestedChunkCount)
    {
        if (chunks.Length >= requestedChunkCount)
        {
            return;
        }

        var newChunkCount = chunks.Length;
        while (newChunkCount < requestedChunkCount)
        {
            newChunkCount *= GrowFactor;
        }

        var newChunks = new int[newChunkCount];
        chunks.CopyTo(newChunks);
        chunks = newChunks;
    }
}

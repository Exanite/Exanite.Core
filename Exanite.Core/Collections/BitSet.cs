using System;
using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Exanite.Core.Collections;

/// <summary>
/// Represents a set of bits that are set to true.
/// Supports set operations and bitwise operations.
/// Uses an ulong[] as the backing memory.
/// </summary>
/// <remarks>
/// Compared to <see cref="BitArray"/>, this data structure does not assume an upper bound for the possible bit index.
/// This means that this data structure does not support operations such as taking the complement of the bit set.
/// <para/>
/// Shift operations are also not supported.
/// This is because this data structure is designed for storing flags, which do not make sense to bitshift.
/// </remarks>
public class BitSet
{
    private const int DefaultChunkCount = 1;
    private const int GrowFactor = 2;
    private const int BitsPerChunk = sizeof(ulong) * 8;
    private const int Shift = 6;
    private const int Mask = (1 << Shift) - 1;

    private ulong[] chunks;

    /// <summary>
    /// The underlying memory used to store the bits.
    /// </summary>
    public ReadOnlySpan<ulong> Chunks => chunks;

    /// <summary>
    /// Returns the number of true bits.
    /// </summary>
    /// <remarks>
    /// This processes every bit stored in the set.
    /// If you only need to know whether the set is empty, use <see cref="IsEmpty"/> instead.
    /// </remarks>
    public int Count
    {
        get
        {
            var count = 0;

            var span = Chunks;
            foreach (var chunk in span)
            {
                // There's no pop count equivalent for SIMD apparently
                count += BitOperations.PopCount(chunk);
            }

            return count;
        }
    }

    /// <summary>
    /// Returns whether the bitset contains all false bits.
    /// </summary>
    /// <remarks>
    /// This short circuits on the first true bit, so this is faster than <see cref="Count"/>
    /// </remarks>
    public bool IsEmpty
    {
        get
        {
            var span = Chunks;
            if (Vector256.IsHardwareAccelerated && span.Length >= Vector256<ulong>.Count)
            {
                var vectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(span);
                foreach (var vector in vectorSpan)
                {
                    if (vector != Vector256<ulong>.Zero)
                    {
                        return false;
                    }
                }

                span = span[(vectorSpan.Length * Vector256<ulong>.Count)..];
            }
            else if (Vector128.IsHardwareAccelerated && span.Length >= Vector128<ulong>.Count)
            {
                var vectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(span);
                foreach (var vector in vectorSpan)
                {
                    if (vector != Vector128<ulong>.Zero)
                    {
                        return false;
                    }
                }

                span = span[(vectorSpan.Length * Vector128<ulong>.Count)..];
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
            return (chunks[chunkIndex] & (1UL << bit)) != 0;
        }
        set
        {
            var chunkIndex = bitIndex >> Shift;
            EnsureCapacity(chunkIndex + 1);

            var bit = bitIndex & Mask;
            if (value)
            {
                chunks[chunkIndex] |= 1UL << bit;
            }
            else
            {
                chunks[chunkIndex] &= ~(1UL << bit);
            }
        }
    }

    /// <summary>
    /// Creates a new empty bit set.
    /// </summary>
    public BitSet()
    {
        chunks = new ulong[DefaultChunkCount];
    }

    /// <summary>
    /// Creates a new bit set by copying the specified bit set.
    /// </summary>
    public BitSet(BitSet other)
    {
        chunks = new ulong[other.Chunks.Length];
        other.Chunks.CopyTo(chunks);
    }

    /// <summary>
    /// Copies the contents of this bitset to the specified bit set.
    /// </summary>
    public void CopyTo(BitSet other)
    {
        other.EnsureCapacity(Chunks.Length);
        Chunks.CopyTo(other.chunks);
    }

    /// <summary>
    /// Clears the bitset.
    /// Does not resize the underlying array.
    /// </summary>
    public void Clear()
    {
        Array.Clear(chunks);
    }

    /// <summary>
    /// Ensures that the bitset has capacity for at least the number of requested chunks.
    /// </summary>
    public void EnsureCapacity(int requestedChunkCount)
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

        var newChunks = new ulong[newChunkCount];
        chunks.CopyTo(newChunks);
        chunks = newChunks;
    }
}

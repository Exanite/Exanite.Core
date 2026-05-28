using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Collections;

/// <summary>
/// Represents a set of bits that are set to true.
/// Supports set operations, but not all bitwise operations.
/// Uses an ulong[] as the backing memory.
/// </summary>
/// <remarks>
/// Compared to <see cref="BitArray"/>, this data structure does not assume an upper bound for the possible bit index.
/// This means that this data structure does not support operations such as taking the complement of the bit set.
/// <para/>
/// Shift operations are also not supported.
/// This is because this data structure is designed for storing flags, which do not make sense to bitshift.
/// </remarks>
public class BitSet : IEnumerable<int>
{
    private const int DefaultChunkCount = 1;
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

            var bitInChunkI = bitIndex & Mask;
            return (chunks[chunkIndex] & (1UL << bitInChunkI)) != 0;
        }
        set
        {
            var chunkIndex = bitIndex >> Shift;
            EnsureCapacity(chunkIndex + 1);

            var bitInChunkI = bitIndex & Mask;
            if (value)
            {
                chunks[chunkIndex] |= 1UL << bitInChunkI;
            }
            else
            {
                chunks[chunkIndex] &= ~(1UL << bitInChunkI);
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
    /// Creates a new empty bit set with the specified capacity.
    /// This specified capacity will use the next power of two, if it is not already a power of two.
    /// </summary>
    public BitSet(int chunkCapacity)
    {
        if (chunkCapacity == 1)
        {
            chunks = new ulong[chunkCapacity];
        }
        else
        {
            // GetNextPowerOfTwo returns a minimum of 2
            chunks = new ulong[M.GetNextPowerOfTwo(chunkCapacity)];
        }
    }

    /// <summary>
    /// Creates a new bit set by copying the specified bit set.
    /// </summary>
    public BitSet(BitSet other)
    {
        chunks = new ulong[other.Chunks.Length];
        other.Chunks.CopyTo(chunks);
    }

    public bool IsProperSubsetOf(BitSet other)
    {
        return Count < other.Count && IsSubsetOf(other);
    }

    public bool IsProperSupersetOf(BitSet other)
    {
        return Count > other.Count && IsSupersetOf(other);
    }

    public bool IsSubsetOf(BitSet other)
    {
        return other.IsSupersetOf(this);
    }

    public bool IsSupersetOf(BitSet other)
    {
        var selfSpan = Chunks;
        var otherSpan = other.Chunks;

        // Process the overlapping region
        var overlapChunkCount = M.Min(selfSpan.Length, otherSpan.Length);
        {
            var processed = 0;
            if (Vector256.IsHardwareAccelerated && overlapChunkCount >= Vector256<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if ((selfChunk & otherChunk) != otherChunk)
                    {
                        return false;
                    }
                }

                processed += count * Vector256<ulong>.Count;
            }
            else if (Vector128.IsHardwareAccelerated && overlapChunkCount >= Vector128<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if ((selfChunk & otherChunk) != otherChunk)
                    {
                        return false;
                    }
                }

                processed += count * Vector128<ulong>.Count;
            }

            for (var i = processed; i < overlapChunkCount; i++)
            {
                var selfChunk = selfSpan[i];
                var otherChunk = otherSpan[i];
                if ((selfChunk & otherChunk) != otherChunk)
                {
                    return false;
                }
            }
        }

        // Process remaining chunks in other
        // If any remaining chunks are non-zero, then this set is not a superset
        var remainingOtherSpan = otherSpan[overlapChunkCount..];
        {
            if (Vector256.IsHardwareAccelerated && remainingOtherSpan.Length >= Vector256<ulong>.Count)
            {
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(remainingOtherSpan);
                foreach (var otherChunk in otherVectorSpan)
                {
                    if (otherChunk != Vector256<ulong>.Zero)
                    {
                        return false;
                    }
                }

                remainingOtherSpan = remainingOtherSpan[(otherVectorSpan.Length * Vector256<ulong>.Count)..];
            }
            else if (Vector128.IsHardwareAccelerated && remainingOtherSpan.Length >= Vector128<ulong>.Count)
            {
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(remainingOtherSpan);
                foreach (var otherChunk in otherVectorSpan)
                {
                    if (otherChunk != Vector128<ulong>.Zero)
                    {
                        return false;
                    }
                }

                remainingOtherSpan = remainingOtherSpan[(otherVectorSpan.Length * Vector128<ulong>.Count)..];
            }

            foreach (var otherChunk in remainingOtherSpan)
            {
                if (otherChunk != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool Overlaps(BitSet other)
    {
        var selfSpan = Chunks;
        var otherSpan = other.Chunks;

        // Process the overlapping region
        var overlapChunkCount = M.Min(selfSpan.Length, otherSpan.Length);
        {
            var processed = 0;
            if (Vector256.IsHardwareAccelerated && overlapChunkCount >= Vector256<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if ((selfChunk & otherChunk) != Vector256<ulong>.Zero)
                    {
                        return true;
                    }
                }

                processed += count * Vector256<ulong>.Count;
            }
            else if (Vector128.IsHardwareAccelerated && overlapChunkCount >= Vector128<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if ((selfChunk & otherChunk) != Vector128<ulong>.Zero)
                    {
                        return true;
                    }
                }

                processed += count * Vector128<ulong>.Count;
            }

            for (var i = processed; i < overlapChunkCount; i++)
            {
                var selfChunk = selfSpan[i];
                var otherChunk = otherSpan[i];
                if ((selfChunk & otherChunk) != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool SetEquals(BitSet other)
    {
        var selfSpan = Chunks;
        var otherSpan = other.Chunks;

        // Process the overlapping region
        var overlapChunkCount = M.Min(selfSpan.Length, otherSpan.Length);
        {
            var processed = 0;
            if (Vector256.IsHardwareAccelerated && overlapChunkCount >= Vector256<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if (selfChunk != otherChunk)
                    {
                        return false;
                    }
                }

                processed += count * Vector256<ulong>.Count;
            }
            else if (Vector128.IsHardwareAccelerated && overlapChunkCount >= Vector128<ulong>.Count)
            {
                var selfVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(selfSpan);
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(otherSpan);
                var count = M.Min(selfVectorSpan.Length, otherVectorSpan.Length);
                for (var i = 0; i < count; i++)
                {
                    var selfChunk = selfVectorSpan[i];
                    var otherChunk = otherVectorSpan[i];
                    if (selfChunk != otherChunk)
                    {
                        return false;
                    }
                }

                processed += count * Vector128<ulong>.Count;
            }

            for (var i = processed; i < overlapChunkCount; i++)
            {
                var selfChunk = selfSpan[i];
                var otherChunk = otherSpan[i];
                if (selfChunk != otherChunk)
                {
                    return false;
                }
            }
        }

        // Process remaining chunks in other
        // If any remaining chunks are non-zero, then the sets are not set equal
        var remainingOtherSpan = otherSpan[overlapChunkCount..];
        {
            if (Vector256.IsHardwareAccelerated && remainingOtherSpan.Length >= Vector256<ulong>.Count)
            {
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector256<ulong>>(remainingOtherSpan);
                foreach (var otherChunk in otherVectorSpan)
                {
                    if (otherChunk != Vector256<ulong>.Zero)
                    {
                        return false;
                    }
                }

                remainingOtherSpan = remainingOtherSpan[(otherVectorSpan.Length * Vector256<ulong>.Count)..];
            }
            else if (Vector128.IsHardwareAccelerated && remainingOtherSpan.Length >= Vector128<ulong>.Count)
            {
                var otherVectorSpan = MemoryMarshal.Cast<ulong, Vector128<ulong>>(remainingOtherSpan);
                foreach (var otherChunk in otherVectorSpan)
                {
                    if (otherChunk != Vector128<ulong>.Zero)
                    {
                        return false;
                    }
                }

                remainingOtherSpan = remainingOtherSpan[(otherVectorSpan.Length * Vector128<ulong>.Count)..];
            }

            foreach (var otherChunk in remainingOtherSpan)
            {
                if (otherChunk != 0)
                {
                    return false;
                }
            }
        }

        return true;
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
            newChunkCount <<= 1;
        }

        var newChunks = new ulong[newChunkCount];
        chunks.CopyTo(newChunks);
        chunks = newChunks;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator<int> IEnumerable<int>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator : IEnumerator<int>
    {
        private readonly BitSet bitset;

        private int bitI = -1;

        public int Current { get; private set; } = -1;
        object IEnumerator.Current => Current;

        public Enumerator(BitSet bitset)
        {
            this.bitset = bitset;
        }

        public bool MoveNext()
        {
            bitI++;

            var chunkI = bitI >> Shift;
            var bitInChunkI = bitI & Mask;
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
                bitI += BitsPerChunk - bitInChunkI;
                chunkI = bitI >> Shift;
                bitInChunkI = bitI & Mask;
            }

            return false;
        }

        public void Reset() => throw new NotSupportedException();
        public void Dispose() {}
    }
}

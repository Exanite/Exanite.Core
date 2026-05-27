using Exanite.Core.Collections;
using Xunit;

namespace Exanite.Core.Tests.Collections;

public class BitSetTests
{
    [Fact]
    public void CanSetBits()
    {
        var bitset = new BitSet();
        bitset[0] = true;
        bitset[10] = true;
        bitset[1000] = true;

        Assert.True(bitset[0]);
        Assert.True(bitset[10]);
        Assert.True(bitset[1000]);
        Assert.Equal(3, bitset.Count);
    }

    [Fact]
    public void IsEmpty_ReturnsIfAllBitsAreFalse()
    {
        var bitset = new BitSet();

        Assert.True(bitset.IsEmpty);
        Assert.Equal(0, bitset.Count);

        bitset[0] = true;
        Assert.False(bitset.IsEmpty);
        Assert.Equal(1, bitset.Count);

        bitset[0] = false;
        Assert.True(bitset.IsEmpty);
        Assert.Equal(0, bitset.Count);
    }

    [Fact]
    public void Clear_SetsAllBitsToFalse()
    {
        var bitset = new BitSet();

        Assert.True(bitset.IsEmpty);
        Assert.Equal(0, bitset.Count);

        bitset[0] = true;
        Assert.False(bitset.IsEmpty);
        Assert.Equal(1, bitset.Count);

        bitset.Clear();
        Assert.True(bitset.IsEmpty);
        Assert.Equal(0, bitset.Count);
    }

    [Fact]
    public void Clear_DoesNotResizeBackingMemory()
    {
        var bitset = new BitSet();
        bitset[10000] = true;

        var chunkCount = bitset.Chunks.Length;
        bitset.Clear();
        Assert.Equal(chunkCount, bitset.Chunks.Length);
    }

    [Fact]
    public void Count_ReturnsNumberOfTrueBits()
    {
        var bitset = new BitSet();
        for (var i = 0; i < 10000; i++)
        {
            bitset[i * 2] = true;
            Assert.Equal(i + 1, bitset.Count);
        }

        bitset.Clear();
        Assert.True(bitset.IsEmpty);
        Assert.Equal(0, bitset.Count);
    }
}

using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void CanEnumerateCollection()
    {
        var bitset = new BitSet();
        var expected = new List<int>();
        for (var i = 0; i < 10000; i++)
        {
            expected.Add(i * 2);
            bitset[i * 2] = true;
        }

        Assert.Equal(expected, bitset.ToArray());
    }

    [Fact]
    public void CopyConstructor()
    {
        var bitset = new BitSet();
        for (var i = 0; i < 1000; i++)
        {
            bitset[i * 3] = true;
        }

        var other = new BitSet(bitset);

        Assert.Equal(bitset.ToArray(), other.ToArray());
    }

    [Fact]
    public void CopyTo()
    {
        var bitset = new BitSet();
        for (var i = 0; i < 1000; i++)
        {
            bitset[i * 3] = true;
        }

        var other = new BitSet();
        bitset.CopyTo(other);

        Assert.Equal(bitset.ToArray(), other.ToArray());
    }

    [Fact]
    public void SetComparison()
    {
        var a = new BitSet()
        {
            [1] = true,
            [10] = true,
            [123] = true,
            [1234] = true,
        };

        var b = new BitSet()
        {
            [1] = true,
            [10] = true,
            // [123] = true,
            [1234] = true,
        };

        Assert.True(a.IsSupersetOf(b));
        Assert.False(b.IsSupersetOf(a));

        Assert.False(a.IsSubsetOf(b));
        Assert.True(b.IsSubsetOf(a));

        Assert.True(a.IsProperSupersetOf(b));

        // True for Overlaps
        // False for SetEquals
    }

    [Fact]
    public void SetComparison_BigValues()
    {
        // The values are really big here since I wanted to roughly test performance here as well
        var a = new BitSet()
        {
            [1] = true,
            [10] = true,
            [123] = true,
            [1234] = true,
            [12345] = true,
            [123456] = true,
            [1234567] = true,
            [12345678] = true,
            [123456789] = true,
            [1234567890] = true,
        };

        var b = new BitSet()
        {
            [1] = true,
            [10] = true,
            // [123] = true,
            [1234] = true,
            [12345] = true,
            // [123456] = true,
            [1234567] = true,
            [12345678] = true,
            // [123456789] = true,
            [1234567890] = true,
        };

        Assert.True(a.IsSupersetOf(b));
        Assert.False(b.IsSupersetOf(a));

        Assert.False(a.IsSubsetOf(b));
        Assert.True(b.IsSubsetOf(a));

        Assert.True(a.IsProperSupersetOf(b));

        // True for Overlaps
        // False for SetEquals
    }

    [Fact]
    public void SetComparison_WhenBothEmpty()
    {
        var a = new BitSet();
        var b = new BitSet();

        Assert.True(a.IsSupersetOf(b));
        Assert.True(b.IsSupersetOf(a));

        Assert.True(a.IsSubsetOf(b));
        Assert.True(b.IsSubsetOf(a));

        // False for Overlaps
        // True for SetEquals
    }

    [Fact]
    public void SetComparison_AgainstSelf()
    {
        var a = new BitSet()
        {
            [10] = true,
        };

        Assert.True(a.IsSupersetOf(a));
        Assert.True(a.IsSubsetOf(a));

        Assert.False(a.IsProperSubsetOf(a));
        Assert.False(a.IsProperSupersetOf(a));

        // True for Overlaps, SetEquals
    }
}

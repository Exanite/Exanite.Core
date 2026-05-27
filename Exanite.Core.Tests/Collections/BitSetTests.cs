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
    }

    [Fact]
    public void IsEmpty_ReturnsIfAllBitsAreFalse()
    {
        var bitset = new BitSet();

        Assert.True(bitset.IsEmpty);

        bitset[0] = true;
        Assert.False(bitset.IsEmpty);

        bitset[0] = false;
        Assert.True(bitset.IsEmpty);
    }

    [Fact]
    public void Clear()
    {
        var bitset = new BitSet();

        Assert.True(bitset.IsEmpty);

        bitset[0] = true;
        Assert.False(bitset.IsEmpty);

        bitset.Clear();
        Assert.True(bitset.IsEmpty);
    }
}

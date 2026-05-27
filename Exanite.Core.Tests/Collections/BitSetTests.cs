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
}

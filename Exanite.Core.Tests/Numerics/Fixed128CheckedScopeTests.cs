using System;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128CheckedScopeTests
{
    [Fact]
    public void CheckedAddition_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked(Fixed128.MaxValue + Fixed128.One);
        });
    }

    [Fact]
    public void CheckedMultiplication_Throws_OnOverflow()
    {
        var large = Fixed128.MaxValue;
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked(large * large);
        });
    }

    [Fact]
    public void CheckedCast_ToFixed128_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed128)float.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed128)double.MaxValue);
        });
    }

    [Fact]
    public void CheckedCast_FromFixed128_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((short)Fixed128.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((int)Fixed128.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((uint)Fixed128.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((ulong)Fixed128.MinValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)Fixed128.MaxValue);
        });
    }
}

using System;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedCheckedScopeTests
{
    [Fact]
    public void CheckedAddition_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked(Fixed.MaxValue + Fixed.One);
        });
    }

    [Fact]
    public void CheckedMultiplication_Throws_OnOverflow()
    {
        var large = Fixed.Sqrt(Fixed.MaxValue) + 10;
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked(large * large);
        });
    }

    [Fact]
    public void CheckedCast_ToFixed_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)long.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)ulong.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)float.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)double.MaxValue);
        });
    }

    [Fact]
    public void CheckedCast_FromFixed_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((short)Fixed.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((int)Fixed.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((uint)Fixed.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((ulong)Fixed.MinValue);
        });
    }
}

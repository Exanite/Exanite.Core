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
    public void CheckedConversion_LongToFixed_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)long.MaxValue);
        });
    }

    [Fact]
    public void CheckedConversion_FixedToInt_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((int)Fixed.MaxValue);
        });
    }
}

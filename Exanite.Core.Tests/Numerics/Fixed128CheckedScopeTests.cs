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
    public void CheckedConversion_Fixed128ToInt_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((int)Fixed128.MaxValue);
        });
    }

    [Fact]
    public void CheckedConversion_Fixed128ToFixed_Throws_OnOverflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            _ = checked((Fixed)Fixed128.MaxValue);
        });
    }
}

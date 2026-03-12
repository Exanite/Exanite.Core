using System;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128BasicTests
{
    [Theory]
    // ToEven
    [InlineData(1.5, MidpointRounding.ToEven, 2)]
    [InlineData(2.5, MidpointRounding.ToEven, 2)]
    [InlineData(-1.5, MidpointRounding.ToEven, -2)]
    [InlineData(-2.5, MidpointRounding.ToEven, -2)]
    // AwayFromZero
    [InlineData(1.5, MidpointRounding.AwayFromZero, 2)]
    [InlineData(2.5, MidpointRounding.AwayFromZero, 3)]
    [InlineData(-1.5, MidpointRounding.AwayFromZero, -2)]
    [InlineData(-2.5, MidpointRounding.AwayFromZero, -3)]
    // ToZero
    [InlineData(1.9, MidpointRounding.ToZero, 1)]
    [InlineData(-1.9, MidpointRounding.ToZero, -1)]
    // ToNegativeInfinity
    [InlineData(1.9, MidpointRounding.ToNegativeInfinity, 1)]
    [InlineData(-1.1, MidpointRounding.ToNegativeInfinity, -2)]
    // ToPositiveInfinity
    [InlineData(1.1, MidpointRounding.ToPositiveInfinity, 2)]
    [InlineData(-1.9, MidpointRounding.ToPositiveInfinity, -1)]
    public void Round_WithMode_ReturnsExpectedValue(double input, MidpointRounding mode, double expected)
    {
        var value = (Fixed128)input;
        var expectedValue = (Fixed128)expected;

        var result = Fixed128.Round(value, mode);

        Assert.Equal(expectedValue, result);
    }
}

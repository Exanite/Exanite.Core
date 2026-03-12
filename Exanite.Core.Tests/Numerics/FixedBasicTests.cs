using System;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedBasicTests
{
    [Fact]
    public void MinMax_AreSymmetric()
    {
        Assert.Equal(Fixed.MaxValue, -Fixed.MinValue);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(123.456, 1)]
    [InlineData(-123.456, -1)]
    public void Sign_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Sign((Fixed)input));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(-1, -1)]
    [InlineData(123.456, 1)]
    [InlineData(-123.456, -1)]
    public void SignNonZero_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.SignNonZero((Fixed)input));
    }

    [Theory]
    [InlineData(1.5, 1)]
    [InlineData(1.4, 1)]
    [InlineData(3.14159, 3)]
    [InlineData(-3.5, -4)]
    public void Floor_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Floor((Fixed)input));
    }

    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(1.4, 2)]
    [InlineData(3.14159, 4)]
    [InlineData(-3.5, -3)]
    public void Ceiling_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Ceiling((Fixed)input));
    }

    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(1.4, 1)]
    [InlineData(3.14159, 3)]
    [InlineData(-3.5, -4)]
    public void Round_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Round((Fixed)input));
    }

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
        var value = (Fixed)input;
        var expectedValue = (Fixed)expected;

        var result = Fixed.Round(value, mode);

        Assert.Equal(expectedValue, result);
    }
}

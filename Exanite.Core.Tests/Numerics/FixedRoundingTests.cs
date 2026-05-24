using System;
using System.Collections;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedRoundingTests
{
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
    [InlineData(MidpointRounding.ToEven, 1.5, 2)]
    [InlineData(MidpointRounding.ToEven, 2.5, 2)]
    [InlineData(MidpointRounding.ToEven, -1.5, -2)]
    [InlineData(MidpointRounding.ToEven, -2.5, -2)]
    [InlineData(MidpointRounding.AwayFromZero, 1.5, 2)]
    [InlineData(MidpointRounding.AwayFromZero, 2.5, 3)]
    [InlineData(MidpointRounding.AwayFromZero, -1.5, -2)]
    [InlineData(MidpointRounding.AwayFromZero, -2.5, -3)]
    [InlineData(MidpointRounding.ToZero, 1.9, 1)]
    [InlineData(MidpointRounding.ToZero, -1.9, -1)]
    [InlineData(MidpointRounding.ToNegativeInfinity, 1.9, 1)]
    [InlineData(MidpointRounding.ToNegativeInfinity, -1.1, -2)]
    [InlineData(MidpointRounding.ToPositiveInfinity, 1.1, 2)]
    [InlineData(MidpointRounding.ToPositiveInfinity, -1.9, -1)]
    public void Round_WithMode_ReturnsExpectedValue(MidpointRounding mode, double input, double expected)
    {
        var value = (Fixed)input;
        var expectedValue = (Fixed)expected;

        var result = Fixed.Round(value, mode);

        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(0, 2, 0)]
    [InlineData(1.5, 0, 2)]
    [InlineData(1.23456, 0, 1)]
    [InlineData(1.23456, 1, 1.2)]
    [InlineData(1.23456, 2, 1.23)]
    [InlineData(1.23456, 3, 1.234)]
    [InlineData(1.23456, 4, 1.2345)]
    [InlineData(1.23456, 5, 1.2346)]
    [InlineData(1.23456, 6, 1.23456)]
    [InlineData(-1.23456, 0, -1)]
    [InlineData(-1.23456, 1, -1.2)]
    [InlineData(-1.23456, 2, -1.23)]
    [InlineData(-1.23456, 3, -1.234)]
    [InlineData(-1.23456, 4, -1.2345)]
    [InlineData(-1.23456, 5, -1.2346)]
    [InlineData(-1.23456, 6, -1.23456)]
    public void Round_WithDigits_ReturnsExpectedValue(double input, int digits, double expected)
    {
        var value = (Fixed)input;
        var expectedValue = (Fixed)expected;

        var result = Fixed.Round(value, digits);

        RoundingAssertEqual(expectedValue, result);
    }

    private void RoundingAssertEqual(Fixed expected, Fixed actual)
    {
        var comparer = FloatingPointComparer.FromTolerance((decimal)Fixed.Epsilon);
        Assert.True(comparer.Equals((decimal)expected, (decimal)actual), $"""
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }
}

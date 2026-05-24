using System;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128RoundingTests
{
    [Theory]
    [InlineData(1.5)]
    [InlineData(1.4)]
    [InlineData(3.14159)]
    [InlineData(-3.5)]
    public void Floor_ReturnsExpectedValue(decimal input)
    {
        var expected = (Fixed128)decimal.Ceiling(input);
        var result = Fixed128.Ceiling((Fixed128)input);
        RoundingAssertEqual(expected, result);
    }

    [Theory]
    [InlineData(1.5)]
    [InlineData(1.4)]
    [InlineData(3.14159)]
    [InlineData(-3.5)]
    public void Ceiling_ReturnsExpectedValue(decimal input)
    {
        var expected = (Fixed128)decimal.Ceiling(input);
        var result = Fixed128.Ceiling((Fixed128)input);
        RoundingAssertEqual(expected, result);
    }

    [Theory]
    [InlineData(1.5)]
    [InlineData(1.4)]
    [InlineData(3.14159)]
    [InlineData(-3.5)]
    public void Round_ReturnsExpectedValue(decimal input)
    {
        var expected = (Fixed128)decimal.Round(input);
        var result = Fixed128.Round((Fixed128)input);
        RoundingAssertEqual(expected, result);
    }

    [Theory]
    [InlineData(MidpointRounding.ToEven, 1.5)]
    [InlineData(MidpointRounding.ToEven, 2.5)]
    [InlineData(MidpointRounding.ToEven, -1.5)]
    [InlineData(MidpointRounding.ToEven, -2.5)]
    [InlineData(MidpointRounding.AwayFromZero, 1.5)]
    [InlineData(MidpointRounding.AwayFromZero, 2.5)]
    [InlineData(MidpointRounding.AwayFromZero, -1.5)]
    [InlineData(MidpointRounding.AwayFromZero, -2.5)]
    [InlineData(MidpointRounding.ToZero, 1.9)]
    [InlineData(MidpointRounding.ToZero, -1.9)]
    [InlineData(MidpointRounding.ToNegativeInfinity, 1.9)]
    [InlineData(MidpointRounding.ToNegativeInfinity, -1.1)]
    [InlineData(MidpointRounding.ToPositiveInfinity, 1.1)]
    [InlineData(MidpointRounding.ToPositiveInfinity, -1.9)]
    public void Round_WithMode_ReturnsExpectedValue(MidpointRounding mode, decimal input)
    {
        var expected = (Fixed128)decimal.Round(input, 0, mode);
        var result = Fixed128.Round((Fixed128)input, mode);
        RoundingAssertEqual(expected, result);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1.5, 0)]
    [InlineData(1.23456, 0)]
    [InlineData(1.23456, 1)]
    [InlineData(1.23456, 2)]
    [InlineData(1.23456, 3)]
    [InlineData(1.23456, 4)]
    [InlineData(1.23456, 5)]
    [InlineData(1.23456, 6)]
    [InlineData(-1.23456, 0)]
    [InlineData(-1.23456, 1)]
    [InlineData(-1.23456, 2)]
    [InlineData(-1.23456, 3)]
    [InlineData(-1.23456, 4)]
    [InlineData(-1.23456, 5)]
    [InlineData(-1.23456, 6)]
    [InlineData(12.3456, 0)]
    [InlineData(12.3456, 1)]
    [InlineData(12.3456, 2)]
    [InlineData(12.3456, 3)]
    [InlineData(12.3456, 4)]
    [InlineData(12.3456, 5)]
    [InlineData(12.3456, 6)]
    [InlineData(-12.3456, 0)]
    [InlineData(-12.3456, 1)]
    [InlineData(-12.3456, 2)]
    [InlineData(-12.3456, 3)]
    [InlineData(-12.3456, 4)]
    [InlineData(-12.3456, 5)]
    [InlineData(-12.3456, 6)]
    public void Round_WithDigits_ReturnsExpectedValue(decimal input, int digits)
    {
        var expected = (Fixed128)decimal.Round(input, digits);
        var result = Fixed128.Round((Fixed128)input, digits);
        RoundingAssertEqual(expected, result);
    }

    private void RoundingAssertEqual(Fixed128 expected, Fixed128 actual)
    {
        var comparer = FloatingPointComparer.FromTolerance((decimal)Fixed128.Epsilon * 2);
        Assert.True(comparer.Equals((decimal)expected, (decimal)actual), $"""
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }
}

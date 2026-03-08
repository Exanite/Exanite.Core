using System;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedTests
{
    [Fact]
    public void MinMax_AreSymmetric()
    {
        Assert.Equal(Fixed.MaxValue, -Fixed.MinValue);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(-1, 1, -1)]
    [InlineData(314159, 100000, 3.14159)]
    [InlineData(-314159, 100000, -3.14159)]
    public void CreateFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromFraction(numerator, denominator), Fixed.Precision);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void CreateParts_ReturnsExpectedResult_IntOverload(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), Fixed.Precision);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void CreateParts_ReturnsExpectedResult_LongOverload(long integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), Fixed.Precision);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void Fixed_CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed.CreateChecked(input), Fixed.Precision);
    }

    [Fact]
    public void Fixed_CreateChecked_HasCorrect_EndpointBehavior()
    {
        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((decimal)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((decimal)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MinValue * 2);
        });
    }

    [Fact]
    public void Fixed_CreateSaturating_HasCorrect_EndpointBehavior()
    {
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((decimal)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((long)Fixed.MaxValue * 2));

        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((decimal)Fixed.MinValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((long)Fixed.MinValue * 2));
    }

    [Fact]
    public void Fixed_CreateTruncating_HasCorrect_EndpointBehavior()
    {
        // TODO: Probably need to adjust precision checks for these
        Assert.Equal((decimal)Fixed.MinValue / 2, (decimal)Fixed.CreateTruncating((decimal)Fixed.MaxValue * 1.5M));
        Assert.Equal((decimal)Fixed.MinValue / 2, (decimal)Fixed.CreateTruncating((long)Fixed.MaxValue * 1.5));

        Assert.Equal((decimal)Fixed.MaxValue / 2, (decimal)Fixed.CreateTruncating((decimal)Fixed.MinValue * 1.5M));
        Assert.Equal((decimal)Fixed.MaxValue / 2, (decimal)Fixed.CreateTruncating((long)Fixed.MinValue * 1.5));
    }

    // TODO: Tests for converting from Fixed

    [Theory]
    [InlineData(1, 5, 1)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Floor_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Floor(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 2)]
    [InlineData(3, 14159, 4)]
    [InlineData(-3, 5, -3)]
    public void Ceiling_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Ceiling(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Round_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Round(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(2, 0, 1.41421, 0)]
    [InlineData(4, 0, 2, 0)]
    [InlineData(36, 0, 6, 0)]
    [InlineData(72, 0, 8.48528, 0)]
    [InlineData(123456, 0, 351.36306, 0)]
    [InlineData(123456789, 0, 11111.11106, 0)]
    [InlineData(123456789012, 0, 351364.18288, 1)]
    public void Sqrt_ReturnsExpectedValue(long integral, int fractional, double expected, int expectedLostPrecision)
    {
        Assert.Equal(expected, (double)Fixed.Sqrt(Fixed.FromParts(integral, fractional)), Fixed.Precision - expectedLostPrecision);
    }

    [Fact]
    public void Sqrt_OfMaxValue_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.MaxValue), (double)Fixed.Sqrt(Fixed.MaxValue), Fixed.Precision - 2);
    }
}

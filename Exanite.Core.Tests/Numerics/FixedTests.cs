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
    public void FromFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromFraction(numerator, denominator), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0.1)]
    [InlineData(0, 2, 0.2)]
    [InlineData(0, 25, 0.25)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void FromParts_ReturnsExpectedResult_IntOverload(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0.1)]
    [InlineData(0, 2, 0.2)]
    [InlineData(0, 25, 0.25)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void CreateParts_ReturnsExpectedResult_LongOverload(long integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed.CreateChecked(input), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void CreateChecked_HasCorrect_EndpointBehavior()
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
    public void CreateSaturating_HasCorrect_EndpointBehavior()
    {
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((decimal)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((long)Fixed.MaxValue * 2));

        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((decimal)Fixed.MinValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((long)Fixed.MinValue * 2));
    }

    [Fact]
    public void Other_GenericCreate_ReturnsCorrectValue()
    {
        // This intentionally only tests for normal circumstances (no overflow)
        Assert.Equal(5, byte.CreateChecked((Fixed)5));
        Assert.Equal(5u, uint.CreateChecked((Fixed)5));
        Assert.Equal(5, int.CreateChecked((Fixed)5));
        Assert.Equal(5, float.CreateChecked((Fixed)5));
        Assert.Equal(5, double.CreateChecked((Fixed)5));

        Assert.Equal(5, byte.CreateSaturating((Fixed)5));
        Assert.Equal(5u, uint.CreateSaturating((Fixed)5));
        Assert.Equal(5, int.CreateSaturating((Fixed)5));
        Assert.Equal(5, float.CreateSaturating((Fixed)5));
        Assert.Equal(5, double.CreateSaturating((Fixed)5));

        Assert.Equal(5, byte.CreateTruncating((Fixed)5));
        Assert.Equal(5u, uint.CreateTruncating((Fixed)5));
        Assert.Equal(5, int.CreateTruncating((Fixed)5));
        Assert.Equal(5, float.CreateTruncating((Fixed)5));
        Assert.Equal(5, double.CreateTruncating((Fixed)5));
    }

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
    [InlineData(0, 0, 0)]
    [InlineData(0, 25, 0.5)]
    [InlineData(2, 0, 1.41421)]
    [InlineData(4, 0, 2)]
    [InlineData(36, 0, 6)]
    [InlineData(72, 0, 8.48528)]
    [InlineData(123456, 0, 351.36306)]
    [InlineData(123456789, 0, 11111.11106)]
    [InlineData(123456789012, 0, 351364.18288)]
    public void Sqrt_ReturnsExpectedValue(long integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.Sqrt(Fixed.FromParts(integral, fractional)), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            Assert.Equal(double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
        }
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_ForLessThanOne()
    {
        var current = 1.0;
        var multiplier = 0.905;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            Assert.Equal(double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision - 2));
        }
    }

    [Fact]
    public void Sqrt_OfMaxValue_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.MaxValue), (double)Fixed.Sqrt(Fixed.MaxValue), FloatingPointComparer.FromPrecision(Fixed.Precision - 2));
    }
}

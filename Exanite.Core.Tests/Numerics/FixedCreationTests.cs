using System;
using Exanite.Core.Numerics;
using Exanite.Core.Runtime;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedCreationTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(-1, 1, -1)]
    [InlineData(314159, 100000, 3.14159)]
    [InlineData(-314159, 100000, -3.14159)]
    public void FromFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromFraction(numerator, denominator), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, 1, 1, 0.1)]
    [InlineData(0, 2, 1, 0.2)]
    [InlineData(0, 25, 2, 0.25)]
    [InlineData(1, 0, 0, 1)]
    [InlineData(-1, 0, 0, -1)]
    [InlineData(1, 1, 1, 1.1)]
    [InlineData(-1, 1, 1, -1.1)]
    [InlineData(3, 14159, 5, 3.14159)]
    [InlineData(-3, 14159, 5, -3.14159)]
    [InlineData(1234, 567, 3, 1234.567)]
    public void FromDecimal_ReturnsExpectedResult(long integral, int fractional, int decimalPlaces, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromDecimal(integral, fractional, decimalPlaces), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void FromDecimal_Throws_ForExcessiveDecimalPlaces()
    {
        Fixed.FromDecimal(0, 0, 9);

        Assert.Throws<GuardException>(() =>
        {
            Fixed.FromDecimal(0, 0, 10);
        });
    }

    [Fact]
    public void FromDecimal_Throws_WhenIntegralPartWillOverflow()
    {
        Assert.Throws<GuardException>(() =>
        {
            Fixed.FromDecimal((long)Fixed.MaxValue + 1, 0, 0);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed.FromDecimal((long)Fixed.MinValue - 1, 0, 0);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed.CreateChecked(input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
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
            Fixed.CreateChecked((decimal)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((double)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((double)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((float)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((float)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked(Fixed128.MaxValue);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked(Fixed128.MinValue);
        });
    }

    [Fact]
    public void CreateSaturating_HasCorrect_EndpointBehavior()
    {
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((decimal)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((decimal)Fixed.MinValue * 2));

        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((double)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((double)Fixed.MinValue * 2));

        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((float)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((float)Fixed.MinValue * 2));

        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((long)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((long)Fixed.MinValue * 2));

        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating(Fixed128.MaxValue));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating(Fixed128.MinValue));
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
        Assert.Equal(5, decimal.CreateChecked((Fixed)5));

        Assert.Equal(5, byte.CreateSaturating((Fixed)5));
        Assert.Equal(5u, uint.CreateSaturating((Fixed)5));
        Assert.Equal(5, int.CreateSaturating((Fixed)5));
        Assert.Equal(5, float.CreateSaturating((Fixed)5));
        Assert.Equal(5, double.CreateSaturating((Fixed)5));
        Assert.Equal(5, decimal.CreateSaturating((Fixed)5));

        Assert.Equal(5, byte.CreateTruncating((Fixed)5));
        Assert.Equal(5u, uint.CreateTruncating((Fixed)5));
        Assert.Equal(5, int.CreateTruncating((Fixed)5));
        Assert.Equal(5, float.CreateTruncating((Fixed)5));
        Assert.Equal(5, double.CreateTruncating((Fixed)5));
        Assert.Equal(5, decimal.CreateTruncating((Fixed)5));
    }
}

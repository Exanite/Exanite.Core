using System;
using Exanite.Core.Numerics;
using Exanite.Core.Runtime;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128CreationTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(-1, 1, -1)]
    [InlineData(314159, 100000, 3.14159)]
    [InlineData(-314159, 100000, -3.14159)]
    public void FromFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed128.FromFraction(numerator, denominator), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
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
    public void FromDecimal_ReturnsExpectedResult(Int128 integral, int fractional, int decimalPlaces, double expected)
    {
        Assert.Equal(expected, (double)Fixed128.FromDecimal(integral, fractional, decimalPlaces), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void FromDecimal_Throws_ForExcessiveDecimalPlaces()
    {
        Fixed128.FromDecimal(0, 0, 9);

        Assert.Throws<GuardException>(() =>
        {
            Fixed128.FromDecimal(0, 0, 10);
        });
    }

    [Fact]
    public void FromDecimal_Throws_WhenIntegralPartWillOverflow()
    {
        Assert.Throws<GuardException>(() =>
        {
            Fixed128.FromDecimal((Int128)Fixed128.MaxValue + 1, 0, 0);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed128.FromDecimal((Int128)Fixed128.MinValue - 1, 0, 0);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed128.CreateChecked(input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void CreateChecked_HasCorrect_EndpointBehavior()
    {
        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((decimal)Fixed128.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((decimal)Fixed128.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((double)Fixed128.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((double)Fixed128.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((float)Fixed128.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed128.CreateChecked((float)Fixed128.MinValue * 2);
        });
    }

    [Fact]
    public void CreateSaturating_HasCorrect_EndpointBehavior()
    {
        Assert.Equal(Fixed128.MaxValue, Fixed128.CreateSaturating((double)Fixed128.MaxValue * 2));
        Assert.Equal(Fixed128.MinValue, Fixed128.CreateSaturating((double)Fixed128.MinValue * 2));

        Assert.Equal(Fixed128.MaxValue, Fixed128.CreateSaturating((float)Fixed128.MaxValue * 2));
        Assert.Equal(Fixed128.MinValue, Fixed128.CreateSaturating((float)Fixed128.MinValue * 2));
    }

    [Fact]
    public void Other_GenericCreate_ReturnsCorrectValue()
    {
        // This intentionally only tests for normal circumstances (no overflow)
        Assert.Equal(5, byte.CreateChecked((Fixed128)5));
        Assert.Equal(5u, uint.CreateChecked((Fixed128)5));
        Assert.Equal(5, int.CreateChecked((Fixed128)5));
        Assert.Equal(5, float.CreateChecked((Fixed128)5));
        Assert.Equal(5, double.CreateChecked((Fixed128)5));
        Assert.Equal(5, decimal.CreateChecked((Fixed128)5));

        Assert.Equal(5, byte.CreateSaturating((Fixed128)5));
        Assert.Equal(5u, uint.CreateSaturating((Fixed128)5));
        Assert.Equal(5, int.CreateSaturating((Fixed128)5));
        Assert.Equal(5, float.CreateSaturating((Fixed128)5));
        Assert.Equal(5, double.CreateSaturating((Fixed128)5));
        Assert.Equal(5, decimal.CreateSaturating((Fixed128)5));

        Assert.Equal(5, byte.CreateTruncating((Fixed128)5));
        Assert.Equal(5u, uint.CreateTruncating((Fixed128)5));
        Assert.Equal(5, int.CreateTruncating((Fixed128)5));
        Assert.Equal(5, float.CreateTruncating((Fixed128)5));
        Assert.Equal(5, double.CreateTruncating((Fixed128)5));
        Assert.Equal(5, decimal.CreateTruncating((Fixed128)5));
    }
}

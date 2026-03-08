using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedTests
{
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
    public void CreateParts_ReturnsExpectedResult(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), Fixed.Precision);
    }

    [Theory]
    [InlineData(1, 5, 1)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Floor_ReturnsExpectedValue(int integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Floor(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 2)]
    [InlineData(3, 14159, 4)]
    [InlineData(-3, 5, -3)]
    public void Ceiling_ReturnsExpectedValue(int integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Ceiling(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Round_ReturnsExpectedValue(int integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Round(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(2, 0, 1.41421)]
    [InlineData(4, 0, 2)]
    [InlineData(36, 0, 6)]
    [InlineData(72, 0, 8.48528)]
    [InlineData(123456, 0, 351.36306)]
    [InlineData(123456789, 0, 11111.11106)]
    [InlineData(123456789012, 0, 351364.18288)]
    public void Sqrt_ReturnsExpectedValue(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.Sqrt(Fixed.FromParts(integral, fractional)), Fixed.Precision - 1);
    }

    [Fact]
    public void Sqrt_OfMaxValue_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.MaxValue), (double)Fixed.Sqrt(Fixed.MaxValue), Fixed.Precision - 1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed.CreateChecked(input), Fixed.Precision);
    }
}

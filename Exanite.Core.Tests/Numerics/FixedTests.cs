using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(314159, 100000, 3.14159)]
    public void CreateFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromFraction(numerator, denominator), Fixed.Precision);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(3, 14159, 3.14159)]
    public void CreateParts_ReturnsExpectedResult(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), Fixed.Precision);
    }
}

using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedBasicTests
{
    [Fact]
    public void Default_IsZero()
    {
        Assert.Equal(Fixed.Zero, default);
    }

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
}

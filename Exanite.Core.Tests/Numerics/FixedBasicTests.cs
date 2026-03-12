using Exanite.Core.Numerics;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;
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

    [Fact]
    public void Reciprocal_DoesNotOverflow_ForEpsilon()
    {
        var result = Fixed.Reciprocal(Fixed.Epsilon);
        Assert.True(result > Fixed.Zero);
    }
}

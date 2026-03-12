using Exanite.Core.Numerics;
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
    public void Cast_FromFloatingPoint_ToFixed_UsesBankersRounding_ForValuesBetweenEpsilon()
    {
        var epsilon = Fixed.Epsilon;

        Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.0));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.25));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.5));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((double)epsilon * 0.75));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((double)epsilon * 1.25));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((double)epsilon * 1.5));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((double)epsilon * 1.75));

        Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.0f));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.25f));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.5f));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((float)epsilon * 0.75f));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((float)epsilon * 1.25f));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((float)epsilon * 1.5f));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((float)epsilon * 1.75f));

        Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.0M));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.25M));
        Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.5M));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((decimal)epsilon * 0.75M));
        Assert.Equal(Fixed.FromRaw(1), (Fixed)((decimal)epsilon * 1.25M));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((decimal)epsilon * 1.5M));
        Assert.Equal(Fixed.FromRaw(2), (Fixed)((decimal)epsilon * 1.75M));

        checked
        {
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.0));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.25));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((double)epsilon * 0.5));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((double)epsilon * 0.75));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((double)epsilon * 1.25));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((double)epsilon * 1.5));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((double)epsilon * 1.75));

            Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.0f));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.25f));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((float)epsilon * 0.5f));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((float)epsilon * 0.75f));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((float)epsilon * 1.25f));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((float)epsilon * 1.5f));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((float)epsilon * 1.75f));

            Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.0M));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.25M));
            Assert.Equal(Fixed.FromRaw(0), (Fixed)((decimal)epsilon * 0.5M));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((decimal)epsilon * 0.75M));
            Assert.Equal(Fixed.FromRaw(1), (Fixed)((decimal)epsilon * 1.25M));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((decimal)epsilon * 1.5M));
            Assert.Equal(Fixed.FromRaw(2), (Fixed)((decimal)epsilon * 1.75M));
        }
    }
}

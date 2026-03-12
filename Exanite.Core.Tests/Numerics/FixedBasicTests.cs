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

    [Fact]
    public void Reciprocal_IsSameAsStandardDivision_ForGreaterThanOne()
    {
        // This test documents that using Fixed128.Reciprocal is not worth it for Fixed.Reciprocal
        //
        // Note that this test is meant to simulate Fixed.Reciprocal
        // being implemented using Fixed128.Reciprocal so the implicit cast is intended
        var current = 1.0M;
        var multiplier = 1.025M;
        var lessPreciseCount = 0;
        var morePreciseCount = 0;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;

            var doubleResult = 1 / current;
            var naiveResult = 1 / (Fixed)current;
            var specializedResult = (Fixed)Fixed128.Reciprocal((Fixed)current);

            var naiveDifference = M.Abs(doubleResult - (decimal)naiveResult);
            var specializedDifference = M.Abs(doubleResult - (decimal)specializedResult);

            if (specializedDifference < naiveDifference)
            {
                morePreciseCount++;
            }

            if (specializedDifference > naiveDifference)
            {
                lessPreciseCount++;
            }
        }

        Assert.Equal(0, morePreciseCount);
        Assert.Equal(0, lessPreciseCount);
    }
}

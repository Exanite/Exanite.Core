using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedQuantizationTests
{
    [Theory]
    [InlineData(0.00, 0)]
    [InlineData(0.25, 0)]
    [InlineData(0.50, 0)]
    [InlineData(0.75, 1)]
    [InlineData(1.25, 1)]
    [InlineData(1.50, 2)]
    [InlineData(1.75, 2)]
    public void Cast_FromFloatingPoint_ToFixed_UsesBankersRounding_ForValuesBetweenEpsilon(decimal multiplier, int expected)
    {
        var epsilon = (decimal)Fixed.Epsilon;

        Assert.Equal(Fixed.FromRaw(expected), (Fixed)(epsilon * multiplier));
        Assert.Equal(Fixed.FromRaw(expected), (Fixed)(double)(epsilon * multiplier));
        Assert.Equal(Fixed.FromRaw(expected), (Fixed)(float)(epsilon * multiplier));

        checked
        {
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)(epsilon * multiplier));
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)(double)(epsilon * multiplier));
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)(float)(epsilon * multiplier));
        }
    }

    [Theory]
    [InlineData(0.00, 0)]
    [InlineData(0.25, 0)]
    [InlineData(0.50, 0)]
    [InlineData(0.75, 1)]
    [InlineData(1.25, 1)]
    [InlineData(1.50, 2)]
    [InlineData(1.75, 2)]
    public void GenericCreate_UsesBankersRounding_ForValuesBetweenEpsilon_ForFloatingPointInputs(decimal multiplier, int expected)
    {
        var epsilon = (decimal)Fixed.Epsilon;

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked(epsilon * multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating(epsilon * multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating(epsilon * multiplier));

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked((double)(epsilon * multiplier)));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating((double)(epsilon * multiplier)));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating((double)(epsilon * multiplier)));

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked((float)(epsilon * multiplier)));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating((float)(epsilon * multiplier)));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating((float)(epsilon * multiplier)));
    }
}

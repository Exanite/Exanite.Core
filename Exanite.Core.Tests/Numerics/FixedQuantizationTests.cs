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
    public void Cast_FromFloatingPoint_ToFixed_UsesBankersRounding_ForValuesBetweenEpsilon(double multiplier, int expected)
    {
        var epsilon = Fixed.Epsilon;

        Assert.Equal(Fixed.FromRaw(expected), (Fixed)((decimal)epsilon * (decimal)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), (Fixed)((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), (Fixed)((float)epsilon * (float)multiplier));

        checked
        {
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)((decimal)epsilon * (decimal)multiplier));
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)((double)epsilon * (double)multiplier));
            Assert.Equal(Fixed.FromRaw(expected), (Fixed)((float)epsilon * (float)multiplier));
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
    public void GenericCreate_UsesBankersRounding_ForValuesBetweenEpsilon_ForFloatingPointInputs(double multiplier, int expected)
    {
        var epsilon = Fixed.Epsilon;

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked((decimal)epsilon * (decimal)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateChecked((float)epsilon * (float)multiplier));

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating((decimal)epsilon * (decimal)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateSaturating((float)epsilon * (float)multiplier));

        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating((decimal)epsilon * (decimal)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed.FromRaw(expected), Fixed.CreateTruncating((float)epsilon * (float)multiplier));
    }
}

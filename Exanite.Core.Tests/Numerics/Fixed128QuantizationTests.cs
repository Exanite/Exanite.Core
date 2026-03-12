using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128QuantizationTests
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
        var epsilon = Fixed128.Epsilon;

        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((float)epsilon * (float)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((decimal)epsilon * (decimal)multiplier));

        checked
        {
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((float)epsilon * (float)multiplier));
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((double)epsilon * (double)multiplier));
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)((decimal)epsilon * (decimal)multiplier));
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
        var epsilon = Fixed128.Epsilon;

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked((float)epsilon * (float)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked((decimal)epsilon * (decimal)multiplier));

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating((float)epsilon * (float)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating((decimal)epsilon * (decimal)multiplier));

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating((float)epsilon * (float)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating((double)epsilon * (double)multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating((decimal)epsilon * (decimal)multiplier));
    }
}

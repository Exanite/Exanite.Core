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
    public void Cast_FromFloatingPoint_ToFixed128_UsesBankersRounding_ForValuesBetweenEpsilon(decimal multiplier, int expected)
    {
        var epsilon = (decimal)Fixed128.Epsilon;

        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(epsilon * multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(double)(epsilon * multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(float)(epsilon * multiplier));

        checked
        {
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(epsilon * multiplier));
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(double)(epsilon * multiplier));
            Assert.Equal(Fixed128.FromRaw(expected), (Fixed128)(float)(epsilon * multiplier));
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
        var epsilon = (decimal)Fixed128.Epsilon;

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked(epsilon * multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating(epsilon * multiplier));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating(epsilon * multiplier));

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked((double)(epsilon * multiplier)));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating((double)(epsilon * multiplier)));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating((double)(epsilon * multiplier)));

        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateChecked((float)(epsilon * multiplier)));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateSaturating((float)(epsilon * multiplier)));
        Assert.Equal(Fixed128.FromRaw(expected), Fixed128.CreateTruncating((float)(epsilon * multiplier)));
    }
}

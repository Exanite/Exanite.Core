using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128BasicTests
{
    [Fact]
    public void Cast_FromFloatingPoint_ToFixed_UsesBankersRounding_ForValuesBetweenEpsilon()
    {
        var epsilon = Fixed128.Epsilon;

        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.0));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.25));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.5));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((double)epsilon * 0.75));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((double)epsilon * 1.25));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((double)epsilon * 1.5));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((double)epsilon * 1.75));

        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.0f));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.25f));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.5f));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((float)epsilon * 0.75f));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((float)epsilon * 1.25f));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((float)epsilon * 1.5f));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((float)epsilon * 1.75f));

        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.0M));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.25M));
        Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.5M));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((decimal)epsilon * 0.75M));
        Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((decimal)epsilon * 1.25M));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((decimal)epsilon * 1.5M));
        Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((decimal)epsilon * 1.75M));

        checked
        {
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.0));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.25));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((double)epsilon * 0.5));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((double)epsilon * 0.75));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((double)epsilon * 1.25));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((double)epsilon * 1.5));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((double)epsilon * 1.75));

            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.0f));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.25f));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((float)epsilon * 0.5f));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((float)epsilon * 0.75f));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((float)epsilon * 1.25f));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((float)epsilon * 1.5f));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((float)epsilon * 1.75f));

            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.0M));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.25M));
            Assert.Equal(Fixed128.FromRaw(0), (Fixed128)((decimal)epsilon * 0.5M));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((decimal)epsilon * 0.75M));
            Assert.Equal(Fixed128.FromRaw(1), (Fixed128)((decimal)epsilon * 1.25M));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((decimal)epsilon * 1.5M));
            Assert.Equal(Fixed128.FromRaw(2), (Fixed128)((decimal)epsilon * 1.75M));
        }
    }
}

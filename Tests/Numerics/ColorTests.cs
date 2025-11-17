using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class ColorTests
{
    [Fact]
    public void Conversion_FromHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal("#FFFFFFFF", Color.FromHsl(0, 0, 1).ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(0, 0, 0).ToHex());

            Assert.Equal("#FFFFFFFF", Color.FromHsl(180, 0, 1).ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(180, 0, 0).ToHex());

            Assert.Equal("#FF0000FF", Color.FromHsl(0, 1, 0.5f).ToHex());

            Assert.Equal("#204C82FF", Color.FromHsl(213, 0.605f, 0.318f).ToHex());
            Assert.Equal("#7FBC80FF", Color.FromHsl(121, 0.313f, 0.617f).ToHex());
            Assert.Equal("#652D06FF", Color.FromHsl(25, 0.890f, 0.209f).ToHex());
        });
    }

    [Fact]
    public void Conversion_FromAndToHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal("#FFFFFFFF", Color.FromHsl(0, 0, 1).Linear.Hsl.ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(0, 0, 0).Linear.Hsl.ToHex());

            Assert.Equal("#FFFFFFFF", Color.FromHsl(180, 0, 1).Linear.Hsl.ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(180, 0, 0).Linear.Hsl.ToHex());

            Assert.Equal("#FF0000FF", Color.FromHsl(0, 1, 0.5f).Linear.Hsl.ToHex());

            Assert.Equal("#204C82FF", Color.FromHsl(213, 0.605f, 0.318f).Linear.Hsl.ToHex());
            Assert.Equal("#7FBC80FF", Color.FromHsl(121, 0.313f, 0.617f).Linear.Hsl.ToHex());
            Assert.Equal("#652D06FF", Color.FromHsl(25, 0.890f, 0.209f).Linear.Hsl.ToHex());
        });
    }
}

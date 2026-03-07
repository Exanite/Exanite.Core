using System.Collections.Generic;
using System.Numerics;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class ColorTests
{
    [Fact]
    public void Conversion_FromHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            // Grayscale
            Assert.Equal("#FFFFFFFF", Color.FromHsl(0, 0, 1).ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(0, 0, 0).ToHex());

            // Grayscale with non-zero hue
            Assert.Equal("#FFFFFFFF", Color.FromHsl(180, 0, 1).ToHex());
            Assert.Equal("#000000FF", Color.FromHsl(180, 0, 0).ToHex());

            // Simple primary color
            Assert.Equal("#FF0000FF", Color.FromHsl(0, 1, 0.5f).ToHex());

            // Hue wrapping
            Assert.Equal("#FF00FFFF", Color.FromHsl(-60, 1, 0.5f).ToHex());
            Assert.Equal("#FF00FFFF", Color.FromHsl(300, 1, 0.5f).ToHex());

            // Values from https://colordesigner.io/convert/hextohsl
            Assert.Equal("#204C82FF", Color.FromHsl(213.06f, 0.6049f, 0.3176f).ToHex());
            Assert.Equal("#7FBC80FF", Color.FromHsl(120.98f, 0.3128f, 0.6176f).ToHex());
            Assert.Equal("#652D06FF", Color.FromHsl(24.63f, 0.8879f, 0.2098f).ToHex());
        });
    }

    [Fact]
    public void Conversion_ToHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            // Grayscale
            Assert.Equal(new Vector3(0, 0, 1), Color.FromHex("#FFFFFFFF").Hsl.Value.Xyz(), HslValueComparer.Instance);
            Assert.Equal(new Vector3(0, 0, 0), Color.FromHex("#000000FF").Hsl.Value.Xyz(), HslValueComparer.Instance);

            // Simple primary color
            Assert.Equal(new Vector3(0, 1, 0.5f), Color.FromHex("#FF0000FF").Hsl.Value.Xyz(), HslValueComparer.Instance);

            // Hue wrapping
            // Should return [0, 360)
            Assert.Equal(new Vector3(300, 1, 0.5f), Color.FromHex("#FF00FFFF").Hsl.Value.Xyz(), HslValueComparer.Instance);

            // Values from https://colordesigner.io/convert/hextohsl
            Assert.Equal(new Vector3(213.06f, 0.6049f, 0.3176f), Color.FromHex("#204C82FF").Hsl.Value.Xyz(), HslValueComparer.Instance);
            Assert.Equal(new Vector3(120.98f, 0.3128f, 0.6176f), Color.FromHex("#7FBC80FF").Hsl.Value.Xyz(), HslValueComparer.Instance);
            Assert.Equal(new Vector3(24.63f, 0.8879f, 0.2098f), Color.FromHex("#652D06FF").Hsl.Value.Xyz(), HslValueComparer.Instance);
        });
    }

    private class HslValueComparer : IEqualityComparer<Vector3>
    {
        public static readonly HslValueComparer Instance = new();

        public bool Equals(Vector3 a, Vector3 b)
        {
            // Use one digit less than expected precision
            return M.ApproximatelyEquals(a.X, b.X, 0.1f)
                && M.ApproximatelyEquals(a.Y, b.Y, 0.001f)
                && M.ApproximatelyEquals(a.Z, b.Z, 0.001f);
        }

        public int GetHashCode(Vector3 value)
        {
            return 0;
        }
    }
}

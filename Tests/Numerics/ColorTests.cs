using Exanite.Core.Numerics;
using NUnit.Framework;

namespace Exanite.Core.Tests.Numerics;

[TestFixture]
public class ColorTests
{
    [Test]
    public void Conversion_FromHsl_ToSrgb_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Color.FromHsl(0, 0, 1).Srgb, Is.EqualTo(Color.FromHex("#FFFFFF").Srgb));
            Assert.That(Color.FromHsl(0, 0, 0).Srgb, Is.EqualTo(Color.FromHex("#000000").Srgb));

            Assert.That(Color.FromHsl(180, 0, 1).Srgb, Is.EqualTo(Color.FromHex("#FFFFFF").Srgb));
            Assert.That(Color.FromHsl(180, 0, 0).Srgb, Is.EqualTo(Color.FromHex("#000000").Srgb));

            Assert.That(Color.FromHsl(213, 0.605f, 0.318f).Srgb, Is.EqualTo(Color.FromHex("#204C82").Srgb));
            Assert.That(Color.FromHsl(121, 0.313f, 0.617f).Srgb, Is.EqualTo(Color.FromHex("#7FBC80").Srgb));
            Assert.That(Color.FromHsl(25, 0.890f, 0.209f).Srgb, Is.EqualTo(Color.FromHex("#652D06").Srgb));
        });
    }
}

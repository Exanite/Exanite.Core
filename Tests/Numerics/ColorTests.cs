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
            Assert.That(Color.FromHsl(0, 0, 1).ToHex(false), Is.EqualTo("#FFFFFF"));
            Assert.That(Color.FromHsl(0, 0, 1).ToHex(false), Is.EqualTo("#FFFFFF"));
            Assert.That(Color.FromHsl(0, 0, 0).ToHex(false), Is.EqualTo("#000000"));

            Assert.That(Color.FromHsl(180, 0, 1).ToHex(false), Is.EqualTo("#FFFFFF"));
            Assert.That(Color.FromHsl(180, 0, 0).ToHex(false), Is.EqualTo("#000000"));

            Assert.That(Color.FromHsl(0, 1, 0.5f).ToHex(false), Is.EqualTo("#FF0000"));

            Assert.That(Color.FromHsl(213, 0.605f, 0.318f).ToHex(false), Is.EqualTo("#204C82"));
            Assert.That(Color.FromHsl(121, 0.313f, 0.617f).ToHex(false), Is.EqualTo("#7FBC80"));
            Assert.That(Color.FromHsl(25, 0.890f, 0.209f).ToHex(false), Is.EqualTo("#652D06"));
        });
    }
}

using Exanite.Core.Numerics;
using NUnit.Framework;

namespace Exanite.Core.Tests.Numerics;

[TestFixture]
public class ColorTests
{
    [Test]
    public void Conversion_FromHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Color.FromHsl(0, 0, 1).ToHex(), Is.EqualTo("#FFFFFFFF"));
            Assert.That(Color.FromHsl(0, 0, 0).ToHex(), Is.EqualTo("#000000FF"));

            Assert.That(Color.FromHsl(180, 0, 1).ToHex(), Is.EqualTo("#FFFFFFFF"));
            Assert.That(Color.FromHsl(180, 0, 0).ToHex(), Is.EqualTo("#000000FF"));

            Assert.That(Color.FromHsl(0, 1, 0.5f).ToHex(), Is.EqualTo("#FF0000FF"));

            Assert.That(Color.FromHsl(213, 0.605f, 0.318f).ToHex(), Is.EqualTo("#204C82FF"));
            Assert.That(Color.FromHsl(121, 0.313f, 0.617f).ToHex(), Is.EqualTo("#7FBC80FF"));
            Assert.That(Color.FromHsl(25, 0.890f, 0.209f).ToHex(), Is.EqualTo("#652D06FF"));
        });
    }

    [Test]
    public void Conversion_FromAndToHsl_ReturnsExpectedResult()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Color.FromHsl(0, 0, 1).Linear.Hsl.ToHex(), Is.EqualTo("#FFFFFFFF"));
            Assert.That(Color.FromHsl(0, 0, 0).Linear.Hsl.ToHex(), Is.EqualTo("#000000FF"));

            Assert.That(Color.FromHsl(180, 0, 1).Linear.Hsl.ToHex(), Is.EqualTo("#FFFFFFFF"));
            Assert.That(Color.FromHsl(180, 0, 0).Linear.Hsl.ToHex(), Is.EqualTo("#000000FF"));

            Assert.That(Color.FromHsl(0, 1, 0.5f).Linear.Hsl.ToHex(), Is.EqualTo("#FF0000FF"));

            Assert.That(Color.FromHsl(213, 0.605f, 0.318f).Linear.Hsl.ToHex(), Is.EqualTo("#204C82FF"));
            Assert.That(Color.FromHsl(121, 0.313f, 0.617f).Linear.Hsl.ToHex(), Is.EqualTo("#7FBC80FF"));
            Assert.That(Color.FromHsl(25, 0.890f, 0.209f).Linear.Hsl.ToHex(), Is.EqualTo("#652D06FF"));
        });
    }
}

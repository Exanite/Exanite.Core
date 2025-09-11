using System.IO;
using Exanite.Core.Io;
using NUnit.Framework;

namespace Exanite.Core.Tests.Io;

[TestFixture]
public class PathTests
{
    [Test]
    public void AbsolutePath_ToString_ReturnsAbsolutePath()
    {
        var path = new AbsolutePath("HelloWorld");

        Assert.That(Path.IsPathRooted(path), Is.True);
    }

    [Test]
    public void RelativePaths_CanBeJoined()
    {
        var path = new RelativePath("A") / "B" / "C";

        Assert.That(Path.IsPathRooted(path), Is.True);
    }
}

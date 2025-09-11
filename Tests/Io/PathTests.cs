using System;
using System.IO;
using Exanite.Core.Io;
using Exanite.Core.Utilities;
using NUnit.Framework;

namespace Exanite.Core.Tests.Io;

[TestFixture]
public class PathTests
{
    [Test]
    public void AbsolutePath_ToString_ReturnsAbsolutePath()
    {
        var path = new AbsolutePath("Absolute");

        Assert.That(Path.IsPathRooted(path), Is.True);
    }

    [Test]
    public void RelativePaths_CanBeJoined_WithStrings()
    {
        var path = new RelativePath("A") / "B" / "C";

        Assert.That(path.Split().Length, Is.EqualTo(3));
    }

    [Test]
    public void RelativePaths_CanBeJoined_WithRelativePaths()
    {
        var path = new RelativePath("A") / new RelativePath("B") / new RelativePath("C");

        Assert.That(path.Split().Length, Is.EqualTo(3));
    }

    [Test]
    public void RelativePaths_CanBeJoined_WithAbsolutePaths()
    {
        var path = new AbsolutePath("Absolute") / new RelativePath("A") / "B" / "C";

        Assert.That(path.Split().Length, Is.GreaterThan(3));
    }

    [Test]
    public void AbsolutePaths_ThrowsException_WhenJoined_WhenEmpty()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = new AbsolutePath() / "Other";
        });
    }

    [Test]
    public void AbsolutePaths_ThrowsException_WhenJoined_WhenDefault()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = default(AbsolutePath) / "Other";
        });
    }

    [Test]
    public void AbsolutePaths_ThrowsException_WhenCreated_WithEmptyString()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = new AbsolutePath("");
        });
    }

    [Test]
    public void AbsolutePaths_CanBeCreatedFrom_RelativePath()
    {
        Assert.DoesNotThrow(() =>
        {
            _ = new AbsolutePath(".");
        });

        Assert.DoesNotThrow(() =>
        {
            _ = new AbsolutePath(new RelativePath("."));
        });
    }
}

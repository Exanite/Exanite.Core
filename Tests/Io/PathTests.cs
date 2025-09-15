using System;
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
    public void RelativePaths_CanBeJoined_WithAbsolutePath()
    {
        var path = new AbsolutePath("Absolute") / new RelativePath("A") / "B" / "C";

        Assert.That(path.Split().Length, Is.GreaterThan(3));
    }

    [Test]
    public void AbsolutePath_ThrowsException_WhenJoined_WhenEmpty()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = new AbsolutePath() / "Other";
        });
    }

    [Test]
    public void AbsolutePath_ThrowsException_WhenJoined_WhenDefault()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = default(AbsolutePath) / "Other";
        });
    }

    [Test]
    public void AbsolutePath_ThrowsException_WhenCreated_WithEmptyString()
    {
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = new AbsolutePath("");
        });
    }

    [Test]
    public void AbsolutePath_CanBeCreatedFrom_RelativePath()
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

    [Test]
    public void AbsolutePath_CanBeCreatedFrom_Slash()
    {
        AbsolutePath path = default;
        Assert.DoesNotThrow(() =>
        {
            path = new AbsolutePath("/");
        });

        Assert.That(path.IsFolder, Is.True);
    }

    [Test]
    public void AbsolutePath_Parent_EventuallyThrows()
    {
        var path = new AbsolutePath(".");

        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            for (var i = 0; i < 1000; i++)
            {
                path = path.Parent;
            }
        });
    }

    [Test]
    public void AbsolutePath_Name_ReturnsLastSegment()
    {
        var path = new AbsolutePath(".") / "A";
        Assert.That(path.Name, Is.EqualTo(new RelativePath("A")));
    }

    [Test]
    public void AbsolutePath_Name_ThrowWhen_IsRoot()
    {
        var path = new AbsolutePath("/");
        Assert.Throws(Is.AssignableTo<Exception>(), () =>
        {
            _ = path.Name;
        });
    }

    [Test]
    public void GetRelativePathTo_ReturnsCorrectRelativePath()
    {
        var basePath = new AbsolutePath("Path");
        var pathInsideBase = basePath / "A";

        {
            var expected = (RelativePath[])["A"];
            Assert.That(basePath.GetRelativePathTo(pathInsideBase).Split(), Is.EquivalentTo(expected));
        }

        {
            var expected = (RelativePath[])[".."];
            Assert.That(pathInsideBase.GetRelativePathTo(basePath).Split(), Is.EquivalentTo(expected));
        }
    }
}

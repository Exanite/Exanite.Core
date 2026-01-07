using System;
using System.IO;
using Exanite.Core.Io;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class PathTests
{
    [Fact]
    public void AbsolutePath_ToString_ReturnsAbsolutePath()
    {
        var path = new AbsolutePath("Absolute");

        Assert.True(Path.IsPathRooted(path));
    }

    [Fact]
    public void RelativePath_CanBeJoined_WithStrings()
    {
        var path = new RelativePath("A") / "B" / "C";

        Assert.Equal(3, path.Split().Length);
    }

    [Fact]
    public void RelativePath_CanBeJoined_WithRelativePaths()
    {
        var path = new RelativePath("A") / new RelativePath("B") / new RelativePath("C");

        Assert.Equal(3, path.Split().Length);
    }

    [Fact]
    public void RelativePath_CanBeJoined_WithAbsolutePath()
    {
        var path = new AbsolutePath("Absolute") / new RelativePath("A") / "B" / "C";

        Assert.True(path.Split().Length > 3);
    }

    [Fact]
    public void RelativePath_ThrowsException_WhenRooted()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new RelativePath("/home");
        });
    }

    [Fact]
    public void RelativePath_ThrowsException_WhenRooted_OnWindows()
    {
        Assert.SkipWhen(!OperatingSystem.IsWindows(), "Windows only");

        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new RelativePath("C:/Users");
        });
    }

    [Fact]
    public void AbsolutePath_ThrowsException_WhenJoined_WhenEmpty()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new AbsolutePath() / "Other";
        });
    }

    [Fact]
    public void AbsolutePath_ThrowsException_WhenJoined_WhenDefault()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = default(AbsolutePath) / "Other";
        });
    }

    [Fact]
    public void AbsolutePath_ThrowsException_WhenCreated_WithEmptyString()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new AbsolutePath("");
        });
    }

    [Fact]
    public void AbsolutePath_CanBeCreatedFrom_RelativePath()
    {
        _ = new AbsolutePath(".");
        _ = new AbsolutePath(new RelativePath("."));
    }

    [Fact]
    public void AbsolutePath_CanBeCreatedFrom_Slash()
    {
        var path = new AbsolutePath("/");
        Assert.True(path.IsFolder);
    }

    [Fact]
    public void AbsolutePath_Name_ReturnsLastSegment()
    {
        var path = new AbsolutePath(".") / "A";
        Assert.Equal(new RelativePath("A"), path.Name);
    }

    [Fact]
    public void AbsolutePath_Name_ThrowWhen_IsRoot()
    {
        var path = new AbsolutePath("/");
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = path.Name;
        });
    }

    [Fact]
    public void AbsolutePath_IsRoot_IsTrue_WhenIsRoot()
    {
        var path = new AbsolutePath("/");
        Assert.True(path.IsRoot);
    }

    [Fact]
    public void AbsolutePath_HandlesWindowsRootPaths_Correctly()
    {
        Assert.SkipWhen(!OperatingSystem.IsWindows(), "Windows only");

        string[] paths = ["C:/", "D:/"];

        foreach (var path in paths)
        {
            var absolutePath = new AbsolutePath(path);
            Assert.Equal(path, absolutePath.ToString());
            Assert.True(absolutePath.IsRoot);
            Assert.ThrowsAny<Exception>(() =>
            {
                _ = absolutePath.Name;
            });
        }
    }

    [Fact]
    public void GetRelativePathTo_ReturnsCorrectRelativePath()
    {
        var basePath = new AbsolutePath("Path");
        var pathInsideBase = basePath / "A";

        {
            var expected = (RelativePath[])["A"];
            Assert.Equal(expected, basePath.GetRelativePathTo(pathInsideBase).Split());
        }

        {
            var expected = (RelativePath[])[".."];
            Assert.Equal(expected, pathInsideBase.GetRelativePathTo(basePath).Split());
        }
    }
}

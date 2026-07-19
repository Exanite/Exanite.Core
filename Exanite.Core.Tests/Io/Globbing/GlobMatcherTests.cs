using System;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io.Globbing;

public class GlobMatcherTests
{
    [Fact]
    public void CorrectStructure()
    {
        var pattern = "!node_modules/**/assets/*/note\\!_?.txt";
        var globPattern = new GlobPattern(pattern);

        Assert.True(globPattern.IsExclude);
        Assert.Equal(5, globPattern.Segments.Count);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[0]);
        Assert.Equal("node_modules", ((LiteralGlobSegment)globPattern.Segments[0]).Literal);

        Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[1]);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[2]);
        Assert.Equal("assets", ((LiteralGlobSegment)globPattern.Segments[2]).Literal);

        Assert.IsType<PatternGlobSegment>(globPattern.Segments[3]);
        Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[3]).Pattern);

        Assert.IsType<PatternGlobSegment>(globPattern.Segments[4]);
        Assert.Equal("note!_?.txt", ((PatternGlobSegment)globPattern.Segments[4]).Pattern);

        Assert.Equal(pattern, globPattern.ToString());
    }

    [Theory]
    [InlineData("\\!")]
    [InlineData("\\?")]
    [InlineData("\\*")]
    [InlineData("abc")]
    [InlineData("abc!")]
    [InlineData("hello.world")]
    public void Valid(string pattern)
    {
        _ = new GlobPattern(pattern);
        _ = new GlobPattern($"!{pattern}");
    }

    [Theory]
    [InlineData("")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("/")]
    [InlineData(@"\\")]
    [InlineData("abc/")]
    [InlineData("folder//")]
    [InlineData("folder/.")]
    [InlineData("folder/..")]
    [InlineData("folder/../a/b/c")]
    [InlineData("./a/b/c")]
    [InlineData("./a/./b")]
    [InlineData("abc//abc")]
    [InlineData(@"folder\\a\\b\\c")]
    public void NotValid(string pattern)
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new GlobPattern(pattern);
        });

        Assert.ThrowsAny<Exception>(() =>
        {
            _ = new GlobPattern($"!{pattern}");
        });
    }
}

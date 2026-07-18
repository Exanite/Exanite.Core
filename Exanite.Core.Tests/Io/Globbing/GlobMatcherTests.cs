using System;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io.Globbing;

public class GlobMatcherTests
{
    [Fact]
    public void CorrectStructure()
    {
        var pattern = "!node_modules/**/assets/*/*.txt";
        var globPattern = GlobMatcher.Parse(pattern);

        Assert.True(globPattern.IsExclude);
        Assert.Equal(5, globPattern.Segments.Count);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[0]);
        Assert.Equal("node_modules", ((LiteralGlobSegment)globPattern.Segments[0]).Value);

        Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[1]);
        Assert.Equal("**", ((DoubleStarGlobSegment)globPattern.Segments[1]).Value);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[2]);
        Assert.Equal("assets", ((LiteralGlobSegment)globPattern.Segments[2]).Value);

        Assert.IsType<PatternGlobSegment>(globPattern.Segments[3]);
        Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[3]).Value);

        Assert.IsType<PatternGlobSegment>(globPattern.Segments[4]);
        Assert.Equal("*.txt", ((PatternGlobSegment)globPattern.Segments[4]).Value);

        Assert.Equal("!node_modules/**/assets/*/*.txt", globPattern.ToString());
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("hello.world")]
    public void Valid(string pattern)
    {
        _ = GlobMatcher.Parse(pattern);
        _ = GlobMatcher.Parse($"!{pattern}");
    }

    [Theory]
    [InlineData("")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("abc!")]
    [InlineData("abc/")]
    [InlineData("folder//")]
    [InlineData("folder/.")]
    [InlineData("folder/..")]
    [InlineData("folder/../a/b/c")]
    [InlineData("./a/b/c")]
    [InlineData("./a/./b")]
    [InlineData("/")]
    [InlineData("abc//abc")]
    [InlineData(@"folder\\a\\b\\c")]
    [InlineData(@"\\")]
    public void NotValid(string pattern)
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = GlobMatcher.Parse(pattern);
        });

        Assert.ThrowsAny<Exception>(() =>
        {
            _ = GlobMatcher.Parse($"!{pattern}");
        });
    }
}

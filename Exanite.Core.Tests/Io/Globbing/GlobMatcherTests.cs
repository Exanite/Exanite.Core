using System;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io.Globbing;

public class GlobMatcherTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("hello.world")]
    public void CanParseLiteral(string pattern)
    {
        var globPattern = GlobMatcher.Parse(pattern);

        Assert.Equal(1, globPattern.Segments.Count);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[0]);
        Assert.Equal(pattern, ((LiteralGlobSegment)globPattern.Segments[0]).Value);
    }

    [Fact]
    public void CanParseComplex()
    {
        var globPattern = GlobMatcher.Parse("node_modules/**/assets/*/*.txt");

        Assert.Equal(5, globPattern.Segments.Count);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[0]);
        Assert.Equal("node_modules", ((LiteralGlobSegment)globPattern.Segments[0]).Value);

        Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[1]);
        Assert.Equal("**", ((DoubleStarGlobSegment)globPattern.Segments[1]).Value);

        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[2]);
        Assert.Equal("assets", ((LiteralGlobSegment)globPattern.Segments[2]).Value);

        Assert.IsType<StarGlobSegment>(globPattern.Segments[3]);
        Assert.Equal("*", ((StarGlobSegment)globPattern.Segments[3]).Value);

        Assert.IsType<PatternGlobSegment>(globPattern.Segments[4]);
        Assert.Equal("*.txt", ((PatternGlobSegment)globPattern.Segments[4]).Value);

        Assert.Equal("node_modules/**/assets/*/*.txt", globPattern.ToString());
    }

    [Theory]
    [InlineData("..")]
    [InlineData("folder/..")]
    [InlineData("folder/../a/b/c")]
    [InlineData(".")]
    [InlineData("./a/b/c")]
    [InlineData("./a/./b")]
    [InlineData(@"folder\\a\\b\\c")]
    [InlineData(@"\\")]
    public void NotValid(string pattern)
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            _ = GlobMatcher.Parse(pattern);
        });
    }
}

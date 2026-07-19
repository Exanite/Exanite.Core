using System;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class GlobPatternTests
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

    [Fact]
    public void EscapeCharacters()
    {
        {
            var pattern = "**/hello**world #?/*";
            var globPattern = new GlobPattern(pattern);

            Assert.False(globPattern.IsExclude);
            Assert.Equal(3, globPattern.Segments.Count);

            Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[0]);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[1]);
            Assert.Equal("hello*world #?", ((PatternGlobSegment)globPattern.Segments[1]).Pattern);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[2]);
            Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[2]).Pattern);

            Assert.Equal(pattern, globPattern.ToString());
        }

        {
            var pattern = @"**/hello\*\*world #?/*";
            var globPattern = new GlobPattern(pattern);

            Assert.False(globPattern.IsExclude);
            Assert.Equal(3, globPattern.Segments.Count);

            Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[0]);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[1]);
            Assert.Equal("hello**world #?", ((PatternGlobSegment)globPattern.Segments[1]).Pattern);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[2]);
            Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[2]).Pattern);

            Assert.Equal(pattern, globPattern.ToString());
        }

        {
            var pattern = @"**/hello\*\*world #\?/*";
            var globPattern = new GlobPattern(pattern);

            Assert.False(globPattern.IsExclude);
            Assert.Equal(3, globPattern.Segments.Count);

            Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[0]);

            Assert.IsType<LiteralGlobSegment>(globPattern.Segments[1]);
            Assert.Equal("hello**world #?", ((LiteralGlobSegment)globPattern.Segments[1]).Literal);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[2]);
            Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[2]).Pattern);

            Assert.Equal(pattern, globPattern.ToString());
        }

        {
            var pattern = @"**/\hello\*\*world #\?/*";
            var globPattern = new GlobPattern(pattern);

            Assert.False(globPattern.IsExclude);
            Assert.Equal(3, globPattern.Segments.Count);

            Assert.IsType<DoubleStarGlobSegment>(globPattern.Segments[0]);

            Assert.IsType<LiteralGlobSegment>(globPattern.Segments[1]);
            Assert.Equal("hello**world #?", ((LiteralGlobSegment)globPattern.Segments[1]).Literal);

            Assert.IsType<PatternGlobSegment>(globPattern.Segments[2]);
            Assert.Equal("*", ((PatternGlobSegment)globPattern.Segments[2]).Pattern);

            Assert.Equal(pattern, globPattern.ToString());
        }
    }

    [Theory]
    // Escaped chars. We let the OS deal with whether these are valid or not.
    [InlineData(@"\!")]
    [InlineData(@"\?")]
    [InlineData(@"\*")]
    [InlineData(@"\.")]
    [InlineData(@"\..")]
    [InlineData(@"\.\.")]
    [InlineData(@"\\")]
    [InlineData(@"\/")]
    [InlineData("abc")]
    [InlineData("abc!")]
    [InlineData("hello.world")]
    [InlineData(@"folder\\a\\b\\c")] // This is just one segment
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
    [InlineData("abc/")]
    [InlineData("folder//")]
    [InlineData("folder/.")]
    [InlineData("folder/..")]
    [InlineData("folder/../a/b/c")]
    [InlineData("./a/b/c")]
    [InlineData("./a/./b")]
    [InlineData("abc//abc")]
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

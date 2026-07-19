using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class GlobSegmentTests
{
    [Theory]
    // Question mark wildcards
    [InlineData("note?", "note0", true)]
    [InlineData("note?", "note1", true)]
    // Star wildcard
    [InlineData("note*", "note", true)]
    [InlineData("note*", "noted", true)]
    [InlineData("note*", "not", false)]
    [InlineData("note*", "note.txt", true)]
    [InlineData("*n*o*t*e*", "note.txt", true)]
    [InlineData("*.txt", "note.txt", true)]
    // Escaped misc
    [InlineData(@"\n\o\t\e?", "note0", true)]
    [InlineData(@"\n\o\t\e?", "note1", true)]
    public void Pattern_IsMatch(string pattern, string input, bool expected)
    {
        var globPattern = new GlobPattern(pattern);

        Assert.Equal(1, globPattern.Segments.Count);
        Assert.IsType<PatternGlobSegment>(globPattern.Segments[0]);
        Assert.Equal(expected, ((PatternGlobSegment)globPattern.Segments[0]).IsMatch(input));
    }

    [Theory]
    // Escaped question mark
    [InlineData(@"note\?", "note?", true)]
    [InlineData(@"note\?", "note0", false)]
    public void Literal_IsMatch(string pattern, string input, bool expected)
    {
        var globPattern = new GlobPattern(pattern);

        Assert.Equal(1, globPattern.Segments.Count);
        Assert.IsType<LiteralGlobSegment>(globPattern.Segments[0]);
        Assert.Equal(expected, ((LiteralGlobSegment)globPattern.Segments[0]).IsMatch(input));
    }
}

using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class PatternGlobSegmentTests
{
    [Theory]
    // Basic wildcards
    [InlineData("note?", "note0", true)]
    [InlineData("note?", "note1", true)]
    [InlineData("note*", "note.txt", true)]
    [InlineData("*.txt", "note.txt", true)]
    // Escaped question mark
    [InlineData(@"note\?", "note?", true)]
    [InlineData(@"note\?", "note0", false)]
    // Escaped misc
    [InlineData(@"\n\o\t\e\0", "note0", true)]
    public void IsMatch(string pattern, string input, bool expected)
    {
        var globPattern = new GlobPattern(pattern);

        Assert.Equal(1, globPattern.Segments.Count);
        Assert.IsType<PatternGlobSegment>(globPattern.Segments[0]);
        Assert.Equal(expected, ((PatternGlobSegment)globPattern.Segments[0]).IsMatch(input));
    }
}

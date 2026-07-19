using System.Collections.Generic;
using System.Text;
using Exanite.Core.Pooling;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Represents a parsed glob pattern.
/// </summary>
public class GlobPattern
{
    private readonly string pattern;
    private readonly List<GlobSegment> segments = new();

    public bool IsExclude { get; }
    public IReadOnlyList<GlobSegment> Segments => segments;

    public GlobPattern(string pattern)
    {
        this.pattern = pattern;

        using var _ = StringBuilderPool.Acquire(out var builder);

        // This parsing code does not always use GlobConstants for simplicity and performance
        var isInEscape = false;
        var isInPattern = false;
        var lastCharWasStar = false;
        var segmentRawLength = 0;

        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];

            if (i == 0 && c == '!')
            {
                IsExclude = true;
                continue;
            }

            // End of segment
            if (!isInEscape && c == '/')
            {
                OutputSegment(builder, segmentRawLength, isInPattern);
                builder.Clear();
                segmentRawLength = 0;

                isInPattern = false;
                lastCharWasStar = false;
                continue;
            }

            segmentRawLength++;

            // Escape character
            if (!isInEscape && c == '\\')
            {
                isInEscape = true;
                lastCharWasStar = false;
                continue;
            }

            var isPatternCharacter = c is '*' or '?';

            // Escaped or normal character
            if (isInEscape || !isPatternCharacter)
            {
                if (isInPattern && isPatternCharacter)
                {
                    builder.Append('\\');
                }

                builder.Append(c);

                isInEscape = false;
                lastCharWasStar = false;
                continue;
            }

            // Pattern characters
            isInPattern = true;
            if (c == '*')
            {
                if (lastCharWasStar)
                {
                    // Collapse consecutive unescaped stars in patterns
                    continue;
                }

                lastCharWasStar = true;
            }
            else
            {
                lastCharWasStar = false;
            }

            builder.Append(c);
        }

        // Handle final segment
        OutputSegment(builder, segmentRawLength, isInPattern);

        GuardUtility.IsFalse(segments.Count == 0, "Pattern must not have zero segments");
    }

    private void OutputSegment(StringBuilder builder, int rawLength, bool isPattern)
    {
        GuardUtility.IsFalse(builder.Length == 0, "Pattern cannot contain a zero length segment");

        // Double star
        if (isPattern && rawLength == 2 && builder is ['*'])
        {
            segments.Add(DoubleStarGlobSegment.Instance);
            return;
        }

        // Disallow directory references
        if (!isPattern && rawLength == 1 && builder is ['.'])
        {
            GuardUtility.Throw("Pattern cannot contain the following segment: .");
        }

        if (!isPattern && rawLength == 2 && builder is ['.', '.'])
        {
            GuardUtility.Throw("Pattern cannot contain the following segment: ..");
        }

        // Pattern
        if (isPattern)
        {
            segments.Add(new PatternGlobSegment(builder.ToString()));
            return;
        }

        // Literal
        segments.Add(new LiteralGlobSegment(builder.ToString()));
    }

    public override string ToString()
    {
        return pattern;
    }
}
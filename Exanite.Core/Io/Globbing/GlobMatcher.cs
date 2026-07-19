using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exanite.Core.Pooling;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io.Globbing;

public static class GlobConstants
{
    public const string PathSeparator = "/";

    public const string Exclude = "!";

    public const string DoubleStar = "**";

    public const string QuestionMark = "?";

    public const string CurrentFolderReference = ".";
    public const string ParentFolderReference = "..";

    /// <summary>
    /// These characters have special meaning in a path segment.
    /// </summary>
    public static readonly SearchValues<char> SegmentPatternCharacters = SearchValues.Create('*', '?');
}

/// <summary>
/// Finds files matching a specified set of glob patterns.
/// Patterns are case-sensitive for cross-platform consistency.
/// Traversal is optimized by only traversing through relevant folders.
/// <para/>
/// Supported features:
/// <list type="bullet">
///     <item><description><c>/</c> as a path segment separator.</description></item>
///     <item><description><c>\</c> as an escape character. The next character will be matched verbatim.</description></item>
///     <item><description><c>!</c> at the beginning of a pattern for excluding matches.</description></item>
///     <item><description><c>?</c> within a segment for matching exactly one character.</description></item>
///     <item><description><c>*</c> within a segment for matching zero or more characters.</description></item>
///     <item><description><c>**</c> as a segment for matching zero or more folder levels.</description></item>
/// </list>
/// Characters not used by supported features are matched directly against file and folder names.
/// <para/>
/// Unsupported features:
/// <list type="bullet">
///     <item><description><c>\</c> as a path segment separator. Use <c>/</c> instead.</description></item>
///     <item><description><c>.</c> for matching the current folder. Using these will throw an error.</description></item>
///     <item><description><c>..</c> for matching the parent folder. Using these will throw an error.</description></item>
///     <item><description><c>[abc]</c> for matching character sets.</description></item>
///     <item><description><c>[a-z]</c> for matching character ranges.</description></item>
///     <item><description><c>{a,b,c}</c> for matching expanded character sets.</description></item>
///     <item><description>
///         Matching folders in general, such as by using a trailing <c>/</c>.
///         This glob implementation only works on files. For example, use <c>folder/**</c> instead of <c>folder/</c>.
///     </description></item>
/// </list>
/// <para/>
/// </summary>
public class GlobMatcher
{
    private readonly GlobPattern[] patterns;

    public GlobMatcher(IEnumerable<string> patterns)
    {
        this.patterns = patterns.Select(pattern => new GlobPattern(pattern)).ToArray();
    }

    public GlobMatcher(IEnumerable<GlobPattern> patterns)
    {
        this.patterns = patterns.ToArray();
    }
}

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

/// <summary>
/// Matches an exact file or folder name.
/// </summary>
public sealed class LiteralGlobSegment : GlobSegment
{
    public readonly string Literal;

    internal LiteralGlobSegment(string literal)
    {
        Literal = literal;
    }
}

/// <summary>
/// Matches a file or folder name by pattern.
/// Supports <c>?</c> for matching exactly one character,
/// <c>*</c> for matching zero or more characters,
/// and <c>\</c> for escape characters.
/// </summary>
public sealed class PatternGlobSegment : GlobSegment
{
    public readonly string Pattern;

    internal PatternGlobSegment(string pattern)
    {
        Pattern = pattern;
    }
}

/// <summary>
/// Matches any file or folder name, recursively.
/// </summary>
public sealed class DoubleStarGlobSegment : GlobSegment
{
    public static readonly DoubleStarGlobSegment Instance = new();

    private DoubleStarGlobSegment() {}
}

/// <summary>
/// Represents a parsed segment of a glob pattern.
/// </summary>
public class GlobSegment
{
    internal GlobSegment() {}
}

public interface IGlobDirectory
{

}

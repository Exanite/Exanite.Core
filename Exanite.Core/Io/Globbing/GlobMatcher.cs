using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
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

    public static readonly SearchValues<char> SegmentPatternCharacters = SearchValues.Create('*', '?');
    public static readonly SearchValues<char> SegmentBannedCharacters = SearchValues.Create('\\', '!');
}

/// <summary>
/// Finds files matching a specified set of glob patterns.
/// Patterns are case-sensitive for cross-platform consistency.
/// Traversal is optimized by only traversing through relevant folders.
/// <para/>
/// Supported features:
/// <list type="bullet">
///     <item><description><c>/</c> as a path segment separator.</description></item>
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
/// <remarks>
/// Currently, files starting with <c>!</c> cannot be matched. This requires support for escape characters or matching the current character.
/// TODO: This seems easy enough to fix?
/// </remarks>
public class GlobMatcher
{
    private readonly GlobPattern[] patterns;

    public GlobMatcher(IEnumerable<string> patterns)
    {
        this.patterns = patterns.Select(Parse).ToArray();
    }

    public GlobMatcher(IEnumerable<GlobPattern> patterns)
    {
        this.patterns = patterns.ToArray();
    }

    public static GlobPattern Parse(string pattern)
    {
        var patternSpan = pattern.AsSpan();

        var isExclude = false;
        if (patternSpan.StartsWith(GlobConstants.Exclude))
        {
            isExclude = true;
            patternSpan = patternSpan[GlobConstants.Exclude.Length..];
        }

        var segmentRanges = patternSpan.Split(GlobConstants.PathSeparator);
        var results = new List<IGlobSegment>();

        foreach (var segmentRange in segmentRanges)
        {
            var segment = patternSpan[segmentRange];
            GuardUtility.IsFalse(segment.Length == 0, "Pattern cannot contain a zero length segment");

            if (segment is GlobConstants.DoubleStar)
            {
                results.Add(DoubleStarGlobSegment.Instance);
                continue;
            }

            if (segment is GlobConstants.CurrentFolderReference or GlobConstants.ParentFolderReference)
            {
                GuardUtility.Throw($"Pattern cannot contain the following segment: {segment}");
            }

            var hasPatternCharacter = false;
            foreach (var c in segment)
            {
                if (GlobConstants.SegmentBannedCharacters.Contains(c))
                {
                    GuardUtility.Throw($"Pattern cannot contain the following character: {c}");
                }

                if (GlobConstants.SegmentPatternCharacters.Contains(c))
                {
                    hasPatternCharacter = true;
                }
            }

            if (!hasPatternCharacter)
            {
                results.Add(new LiteralGlobSegment(segment.ToString()));
                continue;
            }

            results.Add(new PatternGlobSegment(segment.ToString()));
        }

        GuardUtility.IsFalse(results.Count == 0, "Pattern must not have zero segments");

        return new GlobPattern(results, isExclude);
    }
}

/// <summary>
/// Represents a parsed glob pattern.
/// </summary>
public class GlobPattern
{
    public IReadOnlyList<IGlobSegment> Segments { get; }

    public bool IsExclude { get; }

    public GlobPattern(IEnumerable<IGlobSegment> segments, bool isExclude)
    {
        IsExclude = isExclude;
        Segments = segments.ToArray();
    }

    public override string ToString()
    {
        return $"{(IsExclude ? "!" : "")}{string.Join(GlobConstants.PathSeparator, Segments.Select(segment => segment.Value))}";
    }
}

/// <summary>
/// Matches an exact file or folder name.
/// </summary>
public class LiteralGlobSegment : IGlobSegment
{
    public string Value { get; }

    public LiteralGlobSegment(string value)
    {
        Value = value;
    }
}

/// <summary>
/// Matches a file or folder name by pattern.
/// Supports <c>?</c> for matching exactly one character
/// and <c>*</c> for matching zero or more characters.
/// </summary>
public class PatternGlobSegment : IGlobSegment
{
    public string Value { get; }

    public PatternGlobSegment(string value)
    {
        Value = value;
    }
}

/// <summary>
/// Matches any file or folder name, recursively.
/// </summary>
public class DoubleStarGlobSegment : IGlobSegment
{
    public static readonly DoubleStarGlobSegment Instance = new();

    public string Value => "**";

    private DoubleStarGlobSegment() {}
}

/// <summary>
/// Represents a parsed segment of a glob pattern.
/// </summary>
public interface IGlobSegment
{
    public string Value { get; }
}

public interface IGlobDirectory
{

}

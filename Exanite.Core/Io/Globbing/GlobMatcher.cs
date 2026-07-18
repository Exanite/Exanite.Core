using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io.Globbing;

public static class GlobConstants
{
    public const string FolderSeparator = "/";

    public const string Star = "*";
    public const string DoubleStar = "**";

    public const string QuestionMark = "?";

    public const string CurrentFolderReference = ".";
    public const string ParentFolderReference = "..";

    public static readonly SearchValues<char> PatternCharacters = SearchValues.Create('*');
    public static readonly SearchValues<char> BannedCharacters = SearchValues.Create('\\');
}

/// <summary>
/// Finds files matching the specified glob pattern.
/// </summary>
public class GlobMatcher
{
    public GlobMatcher(IEnumerable<string> patterns)
    {

    }

    public static GlobPattern Parse(string pattern)
    {
        var segments = pattern.Split(GlobConstants.FolderSeparator);
        var results = new List<IGlobSegment>();

        foreach (var segment in segments)
        {
            GuardUtility.IsFalse(segment.Length == 0, "Pattern cannot contain a zero length segment");

            if (segment == GlobConstants.Star)
            {
                results.Add(new StarGlobSegment());
                continue;
            }

            if (segment == GlobConstants.DoubleStar)
            {
                results.Add(new DoubleStarGlobSegment());
                continue;
            }

            if (segment is GlobConstants.CurrentFolderReference or GlobConstants.ParentFolderReference)
            {
                GuardUtility.Throw($"Pattern cannot contain the following segment: {segment}");
            }

            var hasPatternCharacter = false;
            foreach (var c in segment)
            {
                if (GlobConstants.BannedCharacters.Contains(c))
                {
                    GuardUtility.Throw($"Pattern cannot contain the following character: {c}");
                }

                if (GlobConstants.PatternCharacters.Contains(c))
                {
                    hasPatternCharacter = true;
                }
            }

            if (!hasPatternCharacter)
            {
                results.Add(new LiteralGlobSegment(segment));
                continue;
            }

            results.Add(new PatternGlobSegment(segment));
        }

        GuardUtility.IsFalse(results.Count == 0, "Pattern must no");

        return new GlobPattern(results);
    }
}

/// <summary>
/// Represents a parsed glob pattern.
/// </summary>
public class GlobPattern
{
    public IReadOnlyList<IGlobSegment> Segments { get; }

    public GlobPattern(IEnumerable<IGlobSegment> segments)
    {
        Segments = segments.ToArray();
    }

    public override string ToString()
    {
        return string.Join(GlobConstants.FolderSeparator, Segments.Select(segment => segment.Value));
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
/// Supports <c>?</c> for matching one character and <c>*</c> for any number of characters.
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
/// Matches any file or folder name.
/// </summary>
public class StarGlobSegment : IGlobSegment
{
    public string Value => "*";
}

/// <summary>
/// Matches any file or folder name, recursively.
/// </summary>
public class DoubleStarGlobSegment : IGlobSegment
{
    public string Value => "**";
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

public interface IGlobFile
{

}

using System.Buffers;

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
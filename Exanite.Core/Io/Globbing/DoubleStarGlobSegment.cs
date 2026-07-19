namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Matches any file or folder name, recursively.
/// </summary>
public sealed class DoubleStarGlobSegment : GlobSegment
{
    public static readonly DoubleStarGlobSegment Instance = new();

    private DoubleStarGlobSegment() {}
}
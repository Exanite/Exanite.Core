namespace Exanite.Core.Io.Globbing;

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
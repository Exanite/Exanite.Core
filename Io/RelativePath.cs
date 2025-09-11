using System.IO;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io;

/// <summary>
/// Represents a relative path.
/// Relative paths must be joined with an absolute path before being used to access a file or folder in a file system.
/// </summary>
public readonly struct RelativePath
{
    private readonly string path;

    public RelativePath(string path)
    {
        this.path = PathUtility.TrimEndSeparators(PathUtility.Normalize(path));
        AssertIsValid();
    }

    // Conversions

    public static implicit operator string(RelativePath path)
    {
        return path.ToString();
    }

    public static implicit operator RelativePath(string path)
    {
        return new RelativePath(path);
    }

    public AbsolutePath ToAbsolutePath()
    {
        return new AbsolutePath(this);
    }

    // Operators

    public static RelativePath operator /(RelativePath a, string b)
    {
        return a / new RelativePath(b);
    }

    public static RelativePath operator /(RelativePath a, RelativePath b)
    {
        a.AssertIsValid();
        b.AssertIsValid();
        return Path.Join(a, b);
    }

    // Operations

    internal void AssertIsValid()
    {
        AssertUtility.IsTrue(!string.IsNullOrEmpty(path), "Relative path cannot be a null or empty string");
    }

    /// <summary>
    /// Splits the path into path segments.
    /// </summary>
    public RelativePath[] Split()
    {
        return [..PathUtility.TrimSeparators(path).Split(Path.DirectorySeparatorChar)];
    }

    public override string ToString()
    {
        return path;
    }
}

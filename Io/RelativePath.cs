using System.IO;

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
        this.path = path;
    }

    public static implicit operator string(RelativePath path)
    {
        return path.ToString();
    }

    public static implicit operator RelativePath(string path)
    {
        return new RelativePath(path);
    }

    public static RelativePath operator /(RelativePath a, RelativePath b)
    {
        return Path.Join(a, b);
    }

    public static RelativePath operator /(RelativePath a, string b)
    {
        return Path.Join(a, b);
    }

    public override string ToString()
    {
        return path;
    }
}
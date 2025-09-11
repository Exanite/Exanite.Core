using System;
using System.IO;

namespace Exanite.Core.Io;

/// <summary>
/// Represents an absolute path that can be used to access a file or folder in a file system.
/// </summary>
public readonly struct AbsolutePath : IEquatable<AbsolutePath>
{
    private readonly string path;

    public bool Exists => Path.Exists(path);
    public bool IsFile => File.Exists(path);
    public bool IsFolder => Directory.Exists(path);

    public AbsolutePath(string path)
    {
        this.path = Path.GetFullPath(path);
    }

    public bool Equals(AbsolutePath other)
    {
        return path == other.path;
    }

    public override bool Equals(object? obj)
    {
        return obj is AbsolutePath other && Equals(other);
    }

    public override int GetHashCode()
    {
        return path.GetHashCode();
    }

    public static implicit operator string(AbsolutePath path)
    {
        return path.ToString();
    }

    public static implicit operator AbsolutePath(string path)
    {
        return new AbsolutePath(path);
    }

    public static AbsolutePath operator /(AbsolutePath a, string b)
    {
        return Path.Join(a, b);
    }

    public static AbsolutePath operator /(AbsolutePath a, RelativePath b)
    {
        return Path.Join(a, b);
    }

    public override string ToString()
    {
        return path;
    }
}

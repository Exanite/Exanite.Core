using System;
using System.IO;
using Exanite.Core.Utilities;

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

    /// <summary>
    /// Creates an absolute path.
    /// </summary>
    /// <param name="path">An absolute or relative path. Relative paths will be joined with the working directory.</param>
    public AbsolutePath(string path)
    {
        this.path = PathUtility.TrimEndSeparators(PathUtility.Normalize(Path.GetFullPath(path)));
        AssertIsValid();
    }

    /// <summary>
    /// Creates an absolute path.
    /// </summary>
    /// <param name="path">A relative path. This will be joined with the working directory.</param>
    public AbsolutePath(RelativePath path) : this(path.ToString()) {}

    // Conversions

    public static implicit operator string(AbsolutePath path)
    {
        return path.ToString();
    }

    public static implicit operator AbsolutePath(string path)
    {
        return new AbsolutePath(path);
    }

    // Comparisons

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

    // Operators

    public static AbsolutePath operator /(AbsolutePath a, string b)
    {
        return a / new RelativePath(b);
    }

    public static AbsolutePath operator /(AbsolutePath a, RelativePath b)
    {
        a.AssertIsValid();
        b.AssertIsValid();
        return Path.Join(a, b);
    }

    // Operations

    internal void AssertIsValid()
    {
        AssertUtility.IsTrue(!string.IsNullOrEmpty(path), "Absolute path cannot be a null or empty string");
    }

    public RelativePath[] Split()
    {
        return [..PathUtility.TrimSeparators(path).Split(Path.DirectorySeparatorChar)];
    }

    public override string ToString()
    {
        return path;
    }
}

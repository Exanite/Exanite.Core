using System;
using System.Collections.Generic;
using System.IO;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io;

/// <summary>
/// Represents a relative path.
/// Relative paths must be joined with an absolute path before being used to access a file or folder in a file system.
/// </summary>
public readonly struct RelativePath : IEquatable<RelativePath>
{
    private readonly string path;

    /// <summary>
    /// The length of the path in characters.
    /// </summary>
    public int Length => path.Length;

    public RelativePath(string path)
    {
        this.path = PathUtility.Normalize(path);
        AssertIsValid();
    }

    // Conversions

    public static implicit operator ReadOnlySpan<char>(RelativePath path)
    {
        return path.path;
    }

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

    // Comparisons

    public static bool operator ==(RelativePath left, RelativePath right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RelativePath left, RelativePath right)
    {
        return !left.Equals(right);
    }

    public bool Equals(RelativePath other)
    {
        return path == other.path;
    }

    public override bool Equals(object? obj)
    {
        return obj is RelativePath other && Equals(other);
    }

    public override int GetHashCode()
    {
        return path.GetHashCode();
    }

    // Operators

    public static RelativePath operator /(RelativePath a, RelativePath b)
    {
        a.AssertIsValid();
        b.AssertIsValid();
        return $"{a}{Path.AltDirectorySeparatorChar}{b}";
    }

    public static RelativePath operator /(RelativePath a, ReadOnlySpan<RelativePath> paths)
    {
        var result = a;
        foreach (var path in paths)
        {
            result /= path;
        }

        return result;
    }

    public static RelativePath operator /(RelativePath a, IEnumerable<RelativePath> paths)
    {
        var result = a;
        foreach (var path in paths)
        {
            result /= path;
        }

        return result;
    }

    // Operations

    internal void AssertIsValid()
    {
        GuardUtility.IsTrue(!string.IsNullOrEmpty(path), "Relative path cannot be a null or empty string");
        GuardUtility.IsTrue(!path.StartsWith(Path.AltDirectorySeparatorChar) && !path.EndsWith(Path.AltDirectorySeparatorChar), "Relative path cannot start or end with slashes");
    }

    /// <summary>
    /// Splits the path into path segments.
    /// </summary>
    public RelativePath[] Split()
    {
        return [..PathUtility.TrimSeparators(path).Split(Path.AltDirectorySeparatorChar)];
    }

    public override string ToString()
    {
        return path;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exanite.Core.Utilities;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Exanite.Core.Io;

/// <summary>
/// Represents an absolute path that can be used to access a file or folder in a file system.
/// </summary>
public readonly struct AbsolutePath : IEquatable<AbsolutePath>
{
    public static AbsolutePath Root => new("/");
    public static AbsolutePath WorkingDirectory => new(".");

    private readonly string path;

    public bool Exists => Path.Exists(path);
    public bool IsFile => File.Exists(path);
    public bool IsFolder => Directory.Exists(path);

    public bool IsRoot => this == Root;

    /// <summary>
    /// Gets the parent folder of this path.
    /// Throws an error if <see cref="IsRoot"/> is true.
    /// </summary>
    public AbsolutePath Parent
    {
        get
        {
            GuardUtility.IsTrue(!IsRoot, "Already a root path");
            return this / "..";
        }
    }

    /// <summary>
    /// The name of the folder or file, including any extensions.
    /// </summary>
    public RelativePath Name
    {
        get
        {
            var lastSlash = path.LastIndexOf(Path.DirectorySeparatorChar);
            return path[(lastSlash + 1)..];
        }
    }

    /// <summary>
    /// Creates an absolute path.
    /// </summary>
    /// <param name="path">An absolute or relative path. Relative paths will be joined with the working directory.</param>
    public AbsolutePath(string path)
    {
        path = Path.GetFullPath(path);
        path = PathUtility.Normalize(path);
        if (path != "/")
        {
            path = PathUtility.TrimEndSeparators(path);
        }

        this.path = path;

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

    public static AbsolutePath operator /(AbsolutePath a, ReadOnlySpan<RelativePath> paths)
    {
        var result = a;
        foreach (var path in paths)
        {
            result /= path;
        }

        return result;
    }

    public static AbsolutePath operator /(AbsolutePath a, IEnumerable<RelativePath> paths)
    {
        var result = a;
        foreach (var path in paths)
        {
            result /= path;
        }

        return result;
    }

    public FileInfo ToFileInfo()
    {
        return new FileInfo(path);
    }

    public DirectoryInfo ToDirectoryInfo()
    {
        return new DirectoryInfo(path);
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
        GuardUtility.IsTrue(!string.IsNullOrEmpty(path), "Absolute path cannot be a null or empty string");
    }

    /// <summary>
    /// Creates the folder at this path if it does not exist.
    /// </summary>
    public void CreateFolder()
    {
        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Deletes the file at this path if it exists
    /// </summary>
    public void DeleteFile()
    {
        File.Delete(path);
    }

    /// <summary>
    /// Deletes the folder at this path if it exists
    /// </summary>
    public void DeleteFolder()
    {
        Directory.Delete(path, true);
    }

    /// <summary>
    /// Copies the file at this path to the target path.
    /// </summary>
    public void CopyFileTo(AbsolutePath target, bool overwrite = false)
    {
        GuardUtility.IsTrue(Exists, "No file exists at the current path");
        File.Copy(this, target, overwrite);
    }

    /// <summary>
    /// Moves the file at this path to the target path.
    /// </summary>
    public void MoveFileTo(AbsolutePath target, bool overwrite = false)
    {
        GuardUtility.IsTrue(IsFile, "No file exists at the current path");
        File.Move(this, target, overwrite);
    }

    /// <summary>
    /// Gets a relative path from this path to another path.
    /// </summary>
    public RelativePath GetRelativePathTo(AbsolutePath other)
    {
        return Path.GetRelativePath(this, other);
    }

    /// <summary>
    /// Splits the path into path segments.
    /// </summary>
    public RelativePath[] Split()
    {
        return [..PathUtility.TrimSeparators(path).Split(Path.DirectorySeparatorChar)];
    }

    /// <summary>
    /// Gets all files immediately under this path.
    /// </summary>
    public AbsolutePath[] GetAllFilesImmediate()
    {
        return [..Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Select(f => new AbsolutePath(f))];
    }

    /// <summary>
    /// Gets all files recursively under this path.
    /// </summary>
    public AbsolutePath[] GetAllFilesRecursive()
    {
        return [..Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(f => new AbsolutePath(f))];
    }

    /// <summary>
    /// Gets all folders immediately under this path.
    /// </summary>
    public AbsolutePath[] GetAllFoldersImmediate()
    {
        return [..Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Select(f => new AbsolutePath(f))];
    }

    /// <summary>
    /// Gets all folders recursively under this path.
    /// </summary>
    public AbsolutePath[] GetAllFoldersRecursive()
    {
        return [..Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Select(f => new AbsolutePath(f))];
    }

    /// <summary>
    /// Finds files matching a glob pattern using this path as the root path.
    /// Patterns are case-sensitive (for cross-platform consistency).
    /// </summary>
    public AbsolutePath[] GlobFiles(string pattern)
    {
        return GlobFiles([pattern]);
    }

    /// <summary>
    /// Finds files matching a list of glob patterns using this path as the root path.
    /// Patterns are case-sensitive (for cross-platform consistency) and are applied in order.
    /// </summary>
    public AbsolutePath[] GlobFiles(ReadOnlySpan<string> patterns)
    {
        var matcher = new Matcher(StringComparison.Ordinal, true);
        foreach (var pattern in patterns)
        {
            if (pattern.StartsWith("!"))
            {
                matcher.AddExclude(pattern[1..]);
            }
            else
            {
                matcher.AddInclude(pattern);
            }
        }

        var root = this;
        var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(path)));

        return results.Files.Select(f => root / f.Path).ToArray();
    }

    public override string ToString()
    {
        return path;
    }
}

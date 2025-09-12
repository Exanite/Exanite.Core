using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    public static AbsolutePath WorkingDirectory => new(".");

    private readonly string path;

    public bool Exists => Path.Exists(path);
    public bool IsFile => File.Exists(path);
    public bool IsFolder => Directory.Exists(path);

    public bool IsFolderEmpty => ToDirectoryInfo().IsEmpty();

    /// <summary>
    /// Gets the parent folder of this path.
    /// </summary>
    public AbsolutePath Parent
    {
        get
        {
            var parent = this / "..";
            GuardUtility.IsTrue(parent != this, "Already a root path");
            return parent;
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
    /// The length of the path in characters.
    /// </summary>
    public int Length => path.Length;

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
    /// Copies the folder at this path to the target path.
    /// </summary>
    public void CopyFolderTo(AbsolutePath targetFolder, bool overwrite = false, bool deleteMismatches = false)
    {
        var originFolder = this;

        GuardUtility.IsTrue(originFolder.IsFolder, "Origin folder does not exist");
        targetFolder.CreateFolder();

        // Copy files
        var relativeOriginFiles = new HashSet<AbsolutePath>(originFolder.GetAllFilesRecursive().Select(path => originFolder.GetRelativePathTo(path).ToAbsolutePath()));
        var relativeTargetFiles = new HashSet<AbsolutePath>(targetFolder.GetAllFilesRecursive().Select(path => targetFolder.GetRelativePathTo(path).ToAbsolutePath()));
        foreach (var relativeOriginFile in relativeOriginFiles)
        {
            var origin = originFolder / relativeOriginFile;
            var target = targetFolder / relativeOriginFile;

            relativeTargetFiles.Remove(relativeOriginFile);

            if (!target.Exists)
            {
                // Output does not exist - Copy to output
                target.Parent.CreateFolder();
                origin.CopyFileTo(target);

                continue;
            }

            if (overwrite && !FileUtility.AreFilesEqual(origin, target))
            {
                // Output is outdated - Copy to output
                origin.CopyFileTo(target, true);

                continue;
            }

            // Output is up to date - Skip
        }

        if (deleteMismatches)
        {
            foreach (var relativeTargetFile in relativeTargetFiles)
            {
                var targetFile = targetFolder / relativeTargetFile;

                targetFile.DeleteFile();
            }
        }

        // Copy folders
        var relativeOriginFolders = new HashSet<AbsolutePath>(originFolder.GetAllFoldersRecursive().Select(path => originFolder.GetRelativePathTo(path).ToAbsolutePath()));
        var relativeTargetFolders = new HashSet<AbsolutePath>(targetFolder.GetAllFoldersRecursive().Select(path => targetFolder.GetRelativePathTo(path).ToAbsolutePath()));
        foreach (var relativeOriginFolder in relativeOriginFolders)
        {
            var target = targetFolder / relativeOriginFolder;
            relativeTargetFolders.Remove(relativeOriginFolder);

            if (!target.Exists)
            {
                // Output does not exist - Create
                target.CreateFolder();
            }
        }

        if (deleteMismatches)
        {
            foreach (var relativeTargetFolder in relativeTargetFolders.OrderByDescending(path => path.Length))
            {
                var target = targetFolder / relativeTargetFolder;

                // Original does not exist
                GuardUtility.IsTrue(target.IsFolderEmpty, "Expected folder to be empty");
                target.DeleteFolder();
            }
        }
    }

    /// <summary>
    /// Moves the folder at this path to the target path.
    /// </summary>
    public void MoveFolderTo(AbsolutePath targetFolder, bool overwrite = false, bool deleteMismatches = false)
    {
        CopyFolderTo(targetFolder, overwrite, deleteMismatches);
        DeleteFolder();
    }

    /// <summary>
    /// Creates a zip archive at the specified path with the contents of the folder at this path.
    /// </summary>
    /// <param name="archiveFile">The path of the archive file.</param>
    /// <param name="includeFolderInArchive">Should the origin folder be included in the zip archive?</param>
    /// <param name="filter">Used to select which files should be included.</param>
    /// <param name="compressionLevel">The compression level to use.</param>
    /// <param name="archiveCreateMode">Whether to update the existing archive or create a new archive.</param>
    public void ZipTo(
        AbsolutePath archiveFile,
        bool includeFolderInArchive = false,
        Func<RelativePath, bool>? filter = null,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        FileMode archiveCreateMode = FileMode.CreateNew)
    {
        var originFolder = this;

        filter ??= _ => true;

        archiveFile.Parent.CreateFolder();

        using var fileStream = File.Open(archiveFile, archiveCreateMode, FileAccess.ReadWrite);
        using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create);

        foreach (var file in originFolder.GetAllFilesRecursive())
        {
            var relativePath = originFolder.GetRelativePathTo(file);
            if (!filter.Invoke(relativePath))
            {
                continue;
            }

            var entryName = relativePath;
            if (includeFolderInArchive)
            {
                // Include the folder in the archive if requested
                entryName = originFolder.Name / relativePath;
            }

            zipArchive.CreateEntryFromFile(file, entryName, compressionLevel);
        }
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

using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Allow globbing files contained in the OS file system.
/// </summary>
public class FileSystemFolder : IGlobFolder
{
    private static readonly EnumerationOptions EnumerationOptions = new()
    {
        AttributesToSkip = FileAttributes.None,
        IgnoreInaccessible = true,
        ReturnSpecialDirectories = false,
        RecurseSubdirectories = false,
    };

    public string Path { get; private init; } = null!;

    private FileSystemFolder() {}

    public FileSystemFolder(string path)
    {
        Path = new AbsolutePath(path);
    }

    public IGlobFolder GetFolder(string name)
    {
        return new FileSystemFolder()
        {
            Path = $"{Path}/{name}",
        };
    }

    public IEnumerable<string> GetFolders()
    {
        return new FileSystemEnumerable<string>(Path, (ref entry) => entry.FileName.ToString(), EnumerationOptions)
        {
            ShouldIncludePredicate = static (ref entry) => entry.IsDirectory
                && (entry.Attributes & FileAttributes.ReparsePoint) == 0, // Skip symlinks
        };
    }

    public IEnumerable<string> GetFiles()
    {
        return new FileSystemEnumerable<string>(Path, (ref entry) => entry.FileName.ToString(), EnumerationOptions)
        {
            ShouldIncludePredicate = static (ref entry) => !entry.IsDirectory,
        };
    }
}

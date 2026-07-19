using System.Collections.Generic;
using System.IO;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Allow globbing files contained in the OS file system.
/// </summary>
public class FileSystemFolder : IGlobFolder
{
    public string Path { get; }

    public FileSystemFolder(string path)
    {
        Path = new AbsolutePath(path);
    }

    public IGlobFolder GetFolder(string name)
    {
        return new FileSystemFolder($"{Path}/{name}");
    }

    public IEnumerable<string> GetFolders()
    {
        return Directory.EnumerateDirectories(Path);
    }

    public IEnumerable<string> GetFiles()
    {
        return Directory.EnumerateFiles(Path);
    }
}
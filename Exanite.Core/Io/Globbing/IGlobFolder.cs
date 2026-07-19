using System.Collections.Generic;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Represents a folder accessible by a glob matcher.
/// </summary>
public interface IGlobFolder
{
    /// <summary>
    /// The path to this current folder.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a folder directly contained within this folder by exact name.
    /// </summary>
    public IGlobFolder GetFolder(string name);

    /// <summary>
    /// Returns the folders directly contained within this folder.
    /// </summary>
    public IEnumerable<string> GetFolders();

    /// <summary>
    /// Returns the files directly contained within this folder.
    /// </summary>
    public IEnumerable<string> GetFiles();
}
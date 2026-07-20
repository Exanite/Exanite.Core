using System.Collections.Generic;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Represents a folder accessible by a glob matcher.
/// </summary>
public interface IGlobFolder
{
    /// <summary>
    /// The full path to this current folder.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a folder directly contained within this folder by exact name.
    /// </summary>
    /// <remarks>
    /// This should be called only with names returned from <see cref="GetFolders"/>.
    /// Other names are undefined behavior.
    /// </remarks>
    public IGlobFolder GetFolder(string name);

    /// <summary>
    /// Returns the names of the folders directly contained within this folder.
    /// </summary>
    public IEnumerable<string> GetFolders();

    /// <summary>
    /// Returns the names of the files directly contained within this folder.
    /// </summary>
    public IEnumerable<string> GetFiles();
}

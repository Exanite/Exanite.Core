using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Exanite.Core.Io;

namespace Exanite.Core.Utilities;

/// <summary>
/// Utility class for managing folders, files and paths
/// </summary>
public static class FileUtility
{
    /// <summary>
    /// Returns true if the provided folder is empty
    /// </summary>
    public static bool IsEmpty(this DirectoryInfo folder)
    {
        return !folder.EnumerateFileSystemInfos().Any();
    }

    /// <summary>
    /// Checks if the files are equal by comparing their contents. Throws if either file does not exist.
    /// </summary>
    public static bool AreFilesEqual(AbsolutePath pathA, AbsolutePath pathB)
    {
        var fileInfo1 = new FileInfo(pathA);
        var fileInfo2 = new FileInfo(pathB);

        if (fileInfo1.Length != fileInfo2.Length)
        {
            return false;
        }

        using var sha256 = SHA256.Create();
        using var stream1 = File.OpenRead(pathA);
        using var stream2 = File.OpenRead(pathB);

        var hash1 = sha256.ComputeHash(stream1);
        var hash2 = sha256.ComputeHash(stream2);

        return hash1.SequenceEqual(hash2);
    }
}

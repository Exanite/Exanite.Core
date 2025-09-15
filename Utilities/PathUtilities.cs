using System.IO;

namespace Exanite.Core.Utilities;

public static class PathUtility
{
    /// <summary>
    /// Normalizes the path, replacing all directory separators with <see cref="Path.AltDirectorySeparatorChar"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Path.AltDirectorySeparatorChar"/> is a forward slash on Windows, Linux, and Mac
    /// so this ensures consistency on all platforms.
    /// </remarks>
    public static string Normalize(string path)
    {
        return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Removes path separators from the end of a path string.
    /// This can change the meaning of the path.
    /// </summary>
    public static string TrimSeparators(string path)
    {
        // This intentionally does not consider the case where there are consecutive, alternative directory separators
        return path.Trim(Path.DirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Removes path separators from the end of a path string.
    /// This can change the meaning of the path.
    /// </summary>
    public static string TrimStartSeparators(string path)
    {
        // This assumes that the directory separator chars don't alternate
        return path.TrimStart(Path.DirectorySeparatorChar).TrimStart(Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Removes path separators from the end of a path string.
    /// This can change the meaning of the path.
    /// </summary>
    /// <remarks>
    /// The existence of a path separator at the end of a path string
    /// can refer to a path or refer to the contents of that folder.
    /// See: https://stackoverflow.com/questions/980255/should-a-directory-path-variable-end-with-a-trailing-slash
    /// </remarks>
    public static string TrimEndSeparators(string path)
    {
        // This intentionally does not consider the case where there are consecutive, alternative directory separators
        return path.TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar);
    }
}

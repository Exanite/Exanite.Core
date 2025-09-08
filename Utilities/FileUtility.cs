using System.IO;
using System.Linq;

namespace Exanite.Core.Utilities
{
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
    }
}

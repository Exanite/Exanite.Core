using System.IO;
using System.Linq;

namespace Exanite.Core.Utilities
{
    /// <summary>
    ///     Extension methods for various <see cref="System.IO" /> classes
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        ///     Returns true if the provided directory is empty
        /// </summary>
        public static bool IsEmpty(this DirectoryInfo directory)
        {
            return !directory.EnumerateFileSystemInfos().Any();
        }
    }
}
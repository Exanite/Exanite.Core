using System.IO;
using System.Linq;
#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Utility class for managing directories, files and paths
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Returns true if the provided directory is empty
        /// </summary>
        public static bool IsEmpty(this DirectoryInfo directory)
        {
            return !directory.EnumerateFileSystemInfos().Any();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns the provided path relative to the Unity assets folder
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public static string GetAssetsRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return $"Assets/{path.Substring(Application.dataPath.Length).Trim('/')}";
            }

            throw new ArgumentException("Path does not contain the project's assets folder", nameof(path));
        }
#endif
    }
}

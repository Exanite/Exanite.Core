using System.IO;
using System.Linq;
#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

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

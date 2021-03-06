﻿using System;
using UnityEngine;

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Utility class for managing directories, files and paths
    /// </summary>
    public static class FileUtility
    {
#if UNITY_EDITOR
        /// <summary>
        /// Returns the provided path relative to the Unity assets folder
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static string GetAssetsRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return $"Assets/{path.Substring(Application.dataPath.Length).Trim('/')}";
            }
            else
            {
                throw new ArgumentException("Path does not contain the project's assets folder", nameof(path));
            }
        }
#endif
    }
}

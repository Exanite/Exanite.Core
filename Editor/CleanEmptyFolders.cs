#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Cleans empty folders from the Unity assets folder.
    /// <para/>
    /// Useful for version control systems like Git.
    /// </summary>
    public static class CleanEmptyFolders
    {
        /// <summary>
        /// Cleans empty folders from the Unity assets folder.
        /// </summary>
        public static void Run()
        {
            var shouldProceed = EditorUtility.DisplayDialog(
                "Remove empty folders?",
                "This will search through your assets for empty folders and delete them if found. Are you sure you want to do this?",
                "Yes",
                "No");

            if (!shouldProceed)
            {
                return;
            }

            var assetsFolder = new DirectoryInfo(Application.dataPath);

            var emptyFolders = GetEmptyFolders(assetsFolder);

            if (emptyFolders.Count > 0)
            {
                var foldersToRemove = string.Empty;

                foreach (var item in emptyFolders)
                {
                    foldersToRemove += $"\n{item.FullName}";
                }

                var shouldDelete = EditorUtility.DisplayDialog(
                    $"Remove {emptyFolders.Count} empty {(emptyFolders.Count == 1 ? "folder" : "folders")}?",
                    $"Pressing 'Yes' will remove the following folders: {foldersToRemove}",
                    "Yes",
                    "No");

                if (shouldDelete)
                {
                    DeleteFolders(emptyFolders);
                }
                else
                {
                    Debug.Log("Operation canceled by user, nothing was deleted.");
                }
            }
        }

        private static List<DirectoryInfo> GetEmptyFolders(DirectoryInfo directory)
        {
            Debug.Log($"Scanning {directory.FullName} for empty folders.");

            var result = new List<DirectoryInfo>();

            foreach (var subDirectory in directory.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                var files = subDirectory.GetFiles("*.*", SearchOption.AllDirectories).ToList();
                files.RemoveAll(x => x.Extension == ".meta");

                if (files.Count == 0)
                {
                    result.Add(subDirectory);
                }
            }

            Debug.Log($"Found {result.Count} empty {(result.Count == 1 ? "folder" : "folders")}.");

            return result;
        }

        private static void DeleteFolders(List<DirectoryInfo> folders)
        {
            var foldersDeleted = 0;

            folders = folders.OrderByDescending(x => x.FullName.Length).ToList();

            foreach (var folder in folders)
            {
                var metaFile = folder.Parent.GetFiles($"{folder.Name}.meta", SearchOption.TopDirectoryOnly).FirstOrDefault();

                Debug.Log($"Deleting empty folder: {folder.FullName}");
                folder.Delete();

                if (metaFile != null)
                {
                    Debug.Log($"Deleting associated meta file: {metaFile.FullName}");
                    metaFile.Delete();
                }

                foldersDeleted++;
            }

            Debug.Log($"Deleted {foldersDeleted} empty {(foldersDeleted == 1 ? "folder" : "folders")}.");

            AssetDatabase.Refresh();
        }
    }
}
#endif

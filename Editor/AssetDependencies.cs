#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Exanite.Core.Editor
{
    public static class AssetDependencies
    {
        private static Dictionary<string, HashSet<string>>? AssetDependents;

        /// <remarks>
        /// This behaves slightly differently compared to Unity's "Select Dependencies" option.
        /// Instead of adding the dependencies to the selection, this only selects the dependencies of the original selection.
        /// This makes it easier to tell if there are any dependencies since no objects will be selected if there aren't any.
        /// </remarks>
        public static void RunSelectAssetDependencies()
        {
            var selectedAssetPaths = GetSelectedAssetPaths();
            var dependencies = new HashSet<string>();
            foreach (var assetPath in selectedAssetPaths)
            {
                foreach (var dependency in AssetDatabase.GetDependencies(assetPath, true))
                {
                    dependencies.Add(dependency);
                }
            }

            foreach (var assetPath in selectedAssetPaths)
            {
                dependencies.Remove(assetPath);
            }

            Selection.objects = dependencies.Select(path => AssetDatabase.LoadMainAssetAtPath(path)).ToArray();

            var logMessage = $"Found {dependencies.Count} dependencies:\n";
            foreach (var dependent in dependencies.ToList().OrderBy(x => x))
            {
                logMessage += $"{dependent}\n";
            }
            Debug.Log(logMessage);
        }

        /// <remarks>
        /// This behaves slightly differently compared to Unity's "Select Dependencies" option.
        /// Instead of adding the dependencies to the selection, this only selects the dependents of the original selection.
        /// This makes it easier to tell if there are any dependents since no objects will be selected if there aren't any.
        /// </remarks>
        public static void RunSelectAssetDependents()
        {
            var selectedAssetPaths = GetSelectedAssetPaths();
            var dependents = GetDependentAssetPaths(selectedAssetPaths);

            foreach (var assetPath in selectedAssetPaths)
            {
                dependents.Remove(assetPath);
            }

            Selection.objects = dependents.Select(path => AssetDatabase.LoadMainAssetAtPath(path)).ToArray();

            var logMessage = $"Found {dependents.Count} dependents:\n";
            foreach (var dependent in dependents.ToList().OrderBy(x => x))
            {
                logMessage += $"{dependent}\n";
            }
            Debug.Log(logMessage);
        }

        private static HashSet<string> GetSelectedAssetPaths()
        {
            var selectedObjects = Selection.objects;
            var selectedAssetPaths = new HashSet<string>();

            foreach (var selectedObject in selectedObjects)
            {
                var path = AssetDatabase.GetAssetPath(selectedObject);
                if (path != null)
                {
                    selectedAssetPaths.Add(path);
                    if (Directory.Exists(path))
                    {
                        foreach (var assetPath in AssetDatabase.FindAssets("", new [] { path }).Select(guid => AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            selectedAssetPaths.Add(assetPath);
                        }
                    }
                }
            }

            return selectedAssetPaths;
        }

        private static HashSet<string> GetDependentAssetPaths(IEnumerable<string> assetPaths)
        {
            UpdateAssetDependents();

            var results = new HashSet<string>();
            foreach (var assetPath in assetPaths)
            {
                if (AssetDependents!.TryGetValue(assetPath, out var dependents))
                {
                    results.UnionWith(dependents);
                }
            }

            return results;
        }

        internal static void ClearAssetDependentsCache()
        {
            AssetDependents = null;
        }

        private static void UpdateAssetDependents()
        {
            if (AssetDependents != null)
            {
                return;
            }

            AssetDependents = new Dictionary<string, HashSet<string>>();

            var allAssetPaths = AssetDatabase.FindAssets("").Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            for (var i = 0; i < allAssetPaths.Length; i++)
            {
                var assetPath = allAssetPaths[i];
                var dependencies = new HashSet<string>(AssetDatabase.GetDependencies(assetPath, true));
                foreach (var dependency in dependencies)
                {
                    if (!AssetDependents.TryGetValue(dependency, out var dependents))
                    {
                        dependents = new HashSet<string>();
                        AssetDependents[dependency] = dependents;
                    }

                    dependents.Add(assetPath);
                }

                UpdateProgressBar(assetPath, i + 1, allAssetPaths.Length);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void UpdateProgressBar(string currentAssetPath, int current, int total)
        {
            EditorUtility.DisplayProgressBar("Finding asset dependents", $"Building cache ({current} / {total}) - {currentAssetPath}", (float)current / total);
        }
    }
}
#endif

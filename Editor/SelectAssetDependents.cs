using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Windows;

#if UNITY_EDITOR
namespace Exanite.Core.Editor
{
    public static class SelectAssetDependents
    {
        private static Dictionary<string, HashSet<string>> AssetDependents;

        public static void Run()
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

            var dependents = GetDependentAssetPaths(selectedAssetPaths);
            Selection.objects = dependents.Select(path => AssetDatabase.LoadMainAssetAtPath(path)).ToArray();
        }

        public static HashSet<string> GetDependentAssetPaths(IEnumerable<string> assetPaths)
        {
            UpdateAssetDependents();

            var results = new HashSet<string>();
            foreach (var assetPath in assetPaths)
            {
                if (AssetDependents.TryGetValue(assetPath, out var dependents))
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

        private static void UpdateAssetDependents(bool force = false)
        {
            if (AssetDependents != null && !force)
            {
                return;
            }

            AssetDependents = new Dictionary<string, HashSet<string>>();

            var allAssetPaths = AssetDatabase.FindAssets("").Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            foreach (var assetPath in allAssetPaths)
            {
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
            }
        }
    }
}
#endif

using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
namespace Exanite.Core.Editor
{
    public static class FindAssetDependents
    {
        public static void Run()
        {
            var selectedObjects = Selection.objects;
            var selectedAssetPaths = new HashSet<string>();

            foreach (var selectedObject in selectedObjects)
            {
                var path = AssetDatabase.GetAssetPath(selectedObject);
                selectedAssetPaths.Add(path);
            }
        }
    }
}
#endif

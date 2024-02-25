#if UNITY_EDITOR
using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Exanite.Core.Utilities
{
    public static class UnityAssetUtility
    {
        /// <summary>
        /// Returns the override importer of an asset. Returns null when the provided
        /// <see cref="Object"/> is not an asset or does not have an override importer.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around <see cref="AssetDatabase.GetImporterOverride"/> to make it easier to use.
        /// </remarks>
        public static Type GetOverrideImporter(this Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.GetImporterOverride(path);
        }
    }
}
#endif

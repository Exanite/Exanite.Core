#if UNITY_EDITOR
using UnityEditor;

namespace Exanite.Core.Editor
{
    internal class UnityEntrypoints : AssetPostprocessor
    {
        // ReSharper disable once Unity.IncorrectMethodSignature - ReSharper is outdated
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            AssetDependencies.ClearAssetDependentsCache();
        }
    }
}
#endif

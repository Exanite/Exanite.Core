#if UNITY_EDITOR
using UnityEditor;

namespace Exanite.Core.Editor
{
    public static class ReserializeAllAssets
    {
        public static void Run()
        {
            var shouldProceed = EditorUtility.DisplayDialog(
                "Reserialize all assets?",
                "This will call AssetDatabase.ForceReserializeAssets. This is useful for updating all assets to their latest versions, but can take a while. Are you sure you want to do this?",
                "Yes",
                "No");

            if (!shouldProceed)
            {
                return;
            }

            AssetDatabase.ForceReserializeAssets();
        }
    }
}
#endif

#if UNITY_EDITOR
#if !EXANITE_CORE_DISABLE_MENU_ITEMS
using UnityEditor;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Defines all the Unity MenuItems used in this assembly
    /// </summary>
    internal static class MenuItemDefines
    {
        [MenuItem("Tools/Exanite.Core/Clean Empty Folders")]
        public static void CleanEmptyFolders()
        {
            Editor.CleanEmptyFolders.Run();
        }

        [MenuItem("Assets/Select Dependencies (Exanite.Core)", false, priority = 30)]
        public static void SelectAssetDependencies()
        {
            AssetDependencies.RunSelectAssetDependencies();
        }

        [MenuItem("Assets/Select Dependents (Exanite.Core)", false, priority = 31)]
        public static void SelectAssetDependents()
        {
            AssetDependencies.RunSelectAssetDependents();
        }

        [MenuItem("Assets/Select Dependencies (Exanite.Core)", true)]
        [MenuItem("Assets/Select Dependents (Exanite.Core)", true)]
        public static bool SelectAssetsValidate()
        {
            return Selection.objects.Length > 0;
        }

        [MenuItem("Tools/Exanite.Core/Reserialize All Assets")]
        public static void ReserializeAllAssets()
        {
            Editor.ReserializeAllAssets.Run();
        }

#if ODIN_INSPECTOR
        [MenuItem("Tools/Exanite.Core/Scriptable Object Creator")]
        public static void OpenScriptableObjectCreator()
        {
            ScriptableObjectCreator.OpenWindow<ScriptableObjectCreator>();
        }
#endif
    }
}
#endif
#endif

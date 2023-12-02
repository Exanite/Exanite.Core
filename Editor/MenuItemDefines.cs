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

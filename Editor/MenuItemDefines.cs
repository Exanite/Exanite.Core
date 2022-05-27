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
            Editor.CleanEmptyFolders.Clean();
        }

#if ODIN_INSPECTOR
        [MenuItem("Tools/Exanite.Core/Scriptable Object Creator")]
        public static void OpenScriptableObjectCreator()
        {
            ScriptableObjectCreator.OpenWindow();
        }
#endif
    }
}
#endif
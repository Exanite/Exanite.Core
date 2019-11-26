using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Defines all the Unity MenuItems used in this assembly
    /// </summary>
    internal static class MenuItemDefines
    {
        [MenuItem("Exanite/Core/Scriptable Object Creator", priority = 0)]
        public static void OpenScriptableObjectCreator()
        {
            ScriptableObjectCreator.OpenWindow();
        }

        [MenuItem("Exanite/Core/Clean Empty Folders", priority = 20)]
        public static void CleanEmptyFolders()
        {
            Editor.CleanEmptyFolders.Clean();
        }
    }
}

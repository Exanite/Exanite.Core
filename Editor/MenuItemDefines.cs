﻿using UnityEditor;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Defines all the Unity MenuItems used in this assembly
    /// </summary>
    internal static class MenuItemDefines
    {
        [MenuItem("Tools/Exanite/Core/Scriptable Object Creator", priority = 0)]
        public static void OpenScriptableObjectCreator()
        {
            ScriptableObjectCreator.OpenWindow();
        }

        [MenuItem("Tools/Exanite/Core/Clean Empty Folders", priority = 20)]
        public static void CleanEmptyFolders()
        {
            Editor.CleanEmptyFolders.Clean();
        }
    }
}

using UnityEngine;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Contains miscellaneous helper methods for Unity
    /// </summary>
    public static class UnityHelper
    {
        /// <summary>
        /// Calls Destroy in Play mode and calls DestroyImmediate in Edit mode
        /// </summary>
        public static void SafeDestroy(Object obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Logs the <paramref name="variableName"/> and <paramref name="value"/> formatted as 'name: value' to the Unity console
        /// </summary>
        public static void LogVariable(string variableName, object value)
        {
            Debug.Log($"{variableName}: {value}");
        }
    } 
}

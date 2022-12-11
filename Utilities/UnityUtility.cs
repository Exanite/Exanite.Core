using UnityEngine;

namespace Exanite.Core.Utilities
{
    /// <summary>
    ///     Utility class for miscellaneous Unity methods.
    /// </summary>
    public static class UnityUtility
    {
        /// <summary>
        ///     Calls Destroy in Play mode and calls DestroyImmediate in Edit
        ///     mode. Unsafe because this can destroy assets.
        /// </summary>
        public static void UnsafeDestroy(Object obj)
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
        ///     Logs the <paramref name="name"/> and <paramref name="value"/>
        ///     formatted as 'name: value' to the Unity console.
        /// </summary>
        public static void LogVariable(string name, object value)
        {
            Debug.Log($"{name}: {value}");
        }
    }
}

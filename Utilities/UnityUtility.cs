#if UNITY_2021_3_OR_NEWER
using UnityEngine;

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Utility class for miscellaneous Unity methods.
    /// </summary>
    public static class UnityUtility
    {
        /// <summary>
        /// Calls Destroy in Play mode and calls DestroyImmediate in Edit
        /// mode. Unsafe because this can destroy assets.
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
    }
}
#endif

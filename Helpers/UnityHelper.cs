using UnityEngine;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Miscellaneous helper class for Unity
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
    } 
}

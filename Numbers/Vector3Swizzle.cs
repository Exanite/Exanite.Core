#if UNITY_2021_3_OR_NEWER
using System;
using UnityEngine;

namespace Exanite.Core.Numbers
{
    /// <summary>
    /// Defines how a <see cref="Vector3"/> value should be swizzled.
    /// </summary>
    [Serializable]
    public enum Vector3Swizzle
    {
        /// <summary>
        /// Keeps the component positions at XYZ.
        /// </summary>
        XYZ,

        /// <summary>
        /// Swaps the component positions from XYZ to XZY.
        /// </summary>
        XZY,

        /// <summary>
        /// Swaps the component positions from XYZ to YXZ.
        /// </summary>
        YXZ,

        /// <summary>
        /// Swaps the component positions from XYZ to YZX.
        /// </summary>
        YZX,

        /// <summary>
        /// Swaps the component positions from XYZ to ZXY.
        /// </summary>
        ZXY,

        /// <summary>
        /// Swaps the component positions from XYZ to ZYX.
        /// </summary>
        ZYX,
    }
}
#endif

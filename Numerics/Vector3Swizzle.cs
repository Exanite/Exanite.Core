using System;

namespace Exanite.Core.Numerics
{
    /// <summary>
    /// Defines how the components of a Vector3 should be swizzled.
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

using System;

namespace Exanite.Core.Numerics
{
    /// <summary>
    /// Defines how the components of a Vector3 should be swizzled.
    /// </summary>
    [Serializable]
    public enum Vector3Swizzle
    {
        // ReSharper disable InconsistentNaming
        
        /// <summary>
        /// Keeps the component positions at XYZ.
        /// </summary>
        XYZ = 0,

        /// <summary>
        /// Swaps the component positions from XYZ to XZY.
        /// </summary>
        XZY = 1,

        /// <summary>
        /// Swaps the component positions from XYZ to YXZ.
        /// </summary>
        YXZ = 2,

        /// <summary>
        /// Swaps the component positions from XYZ to YZX.
        /// </summary>
        YZX = 3,

        /// <summary>
        /// Swaps the component positions from XYZ to ZXY.
        /// </summary>
        ZXY = 4,

        /// <summary>
        /// Swaps the component positions from XYZ to ZYX.
        /// </summary>
        ZYX = 5,
    }
}

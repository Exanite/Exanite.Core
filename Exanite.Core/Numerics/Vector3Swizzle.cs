using System;

namespace Exanite.Core.Numerics;

/// <summary>
/// Defines how the components of a Vector3 should be swizzled.
/// </summary>
[Serializable]
public enum Vector3Swizzle
{
    /// <summary>
    /// Keeps the component positions at XYZ.
    /// </summary>
    Xyz = 0,

    /// <summary>
    /// Swaps the component positions from XYZ to XZY.
    /// </summary>
    Xzy = 1,

    /// <summary>
    /// Swaps the component positions from XYZ to YXZ.
    /// </summary>
    Yxz = 2,

    /// <summary>
    /// Swaps the component positions from XYZ to YZX.
    /// </summary>
    Yzx = 3,

    /// <summary>
    /// Swaps the component positions from XYZ to ZXY.
    /// </summary>
    Zxy = 4,

    /// <summary>
    /// Swaps the component positions from XYZ to ZYX.
    /// </summary>
    Zyx = 5,
}

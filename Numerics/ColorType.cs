namespace Exanite.Core.Numerics;

/// <summary>
/// Specifies the color space representation.
/// </summary>
public enum ColorType
{
    /// <summary>
    /// Standard RGB color space (sRGB).
    /// Commonly used for UI and images.
    /// </summary>
    Srgb = 0,

    /// <summary>
    /// Linear sRGB color space.
    /// Commonly used in rendering and lighting calculations.
    /// </summary>
    Linear,
}
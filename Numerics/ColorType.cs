namespace Exanite.Core.Numerics;

/// <summary>
/// Specifies the color space representation.
/// </summary>
public enum ColorType
{
    /// <summary>
    /// Linear sRGB color space.
    /// Commonly used in rendering and lighting calculations.
    /// </summary>
    Linear = 0,

    /// <summary>
    /// Standard RGB color space (sRGB).
    /// Commonly used for UI and images.
    /// </summary>
    Srgb,

    /// <summary>
    /// HSL color space (hue, saturation, lightness).
    /// Commonly used for color manipulation.
    /// </summary>
    Hsl,
}

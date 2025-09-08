using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Colors

    // sRGB-Linear conversion formulas are from: https://entropymine.com/imageworsener/srgbformula/

    /// <summary>
    /// Converts a sRGB [0, 1] color value to linear [0, 1].
    /// </summary>
    public static float SrgbToLinear(float value)
    {
        if (value <= 0.04045f)
        {
            return value / 12.92f;
        }

        return float.Pow((value + 0.055f) / 1.055f, 2.4f);
    }

    /// <summary>
    /// Converts a linear [0, 1] color value to sRGB [0, 1].
    /// </summary>
    public static float LinearToSrgb(float value)
    {
        if (value <= 0.0031308f)
        {
            return value * 12.92f;
        }

        return float.Pow(value, 1 / 2.4f) * 1.055f - 0.055f;
    }

    /// <summary>
    /// Converts the RGB (XYZ) components from sRGB [0, 1] to linear [0, 1].
    /// </summary>
    public static Vector3 SrgbToLinear(Vector3 srgb)
    {
        return new Vector3(SrgbToLinear(srgb.X), SrgbToLinear(srgb.Y), SrgbToLinear(srgb.Z));
    }

    /// <summary>
    /// Converts the RGB (XYZ) components from linear [0, 1] to sRGB [0, 1].
    /// </summary>
    public static Vector3 LinearToSrgb(Vector3 srgb)
    {
        return new Vector3(LinearToSrgb(srgb.X), LinearToSrgb(srgb.Y), LinearToSrgb(srgb.Z));
    }

    /// <summary>
    /// Converts the RGB (XYZ) components from sRGB [0, 1] to linear [0, 1]. Does not modify the alpha (W) component.
    /// </summary>
    public static Vector4 SrgbToLinear(Vector4 srgb)
    {
        return new Vector4(SrgbToLinear(srgb.X), SrgbToLinear(srgb.Y), SrgbToLinear(srgb.Z), srgb.W);
    }

    /// <summary>
    /// Converts the RGB (XYZ) components from linear [0, 1] to sRGB [0, 1]. Does not modify the alpha (W) component.
    /// </summary>
    public static Vector4 LinearToSrgb(Vector4 srgb)
    {
        return new Vector4(LinearToSrgb(srgb.X), LinearToSrgb(srgb.Y), LinearToSrgb(srgb.Z), srgb.W);
    }

    /// <summary>
    /// Converts a hex color code to sRGB [0, 1].
    /// </summary>
    public static Vector4 HexToSrgb(string hex)
    {
        var color = ColorTranslator.FromHtml(hex); // TODO: Don't use FromHtml

        return new Vector4(color.R, color.G, color.B, color.A) / byte.MaxValue;
    }

    /// <summary>
    /// Converts a hex color code to linear [0, 1].
    /// </summary>
    public static Vector4 HexToLinear(string hex)
    {
        return SrgbToLinear(HexToSrgb(hex));
    }

    #endregion
}

using System;
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
        AssertUtility.IsTrue(hex.Length > 0, "Color code is empty");

        // Remove leading '#'
        if (hex.StartsWith('#'))
        {
            hex = hex.Substring(1);
        }

        // Expand shorthand (#fff or #ffff)
        if (hex.Length == 3 || hex.Length == 4)
        {
            var temp = "";
            foreach (var c in hex)
            {
                temp += new string(c, 2);
            }
            hex = temp;
        }

        // Add default alpha if missing
        if (hex.Length == 6)
        {
            hex += "ff";
        }

        AssertUtility.IsTrue(hex.Length == 8, "Invalid length for input color code");

        // Parse components
        var result = new Vector4();
        for (var i = 0; i < 4; ++i)
        {
            result[i] = Convert.ToInt32(hex.Substring(i * 2, 2), 16);
        }

        return result / byte.MaxValue;
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

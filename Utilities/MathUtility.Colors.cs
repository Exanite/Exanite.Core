using System;
using System.Numerics;
using Exanite.Core.Numerics;
using Exanite.Core.Pooling;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Color conversion

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

        using var _ = StringBuilderPool.Acquire(out var builder);

        builder.EnsureCapacity(8);
        builder.Append(hex);

        // Remove leading '#'
        if (builder[0] == '#')
        {
            builder.Remove(0, 1);
        }

        // Expand shorthand (#fff or #ffff)
        if (builder.Length is 3 or 4)
        {
            var originalLength = builder.Length;
            builder.Length = originalLength * 2;

            for (var i = originalLength - 1; i >= 0; i--)
            {
                var value = builder[i];
                builder[i * 2] = value;
                builder[i * 2 + 1] = value;
            }
        }

        // Add default alpha if missing
        if (builder.Length == 6)
        {
            builder.Append("ff");
        }

        AssertUtility.IsTrue(builder.Length == 8, "Invalid length for input color code");

        // Parse components
        var result = new Vector4();
        for (var i = 0; i < 4; ++i)
        {
            var high = HexCharToInt(builder[i * 2]);
            var low = HexCharToInt(builder[i * 2 + 1]);
            result[i] = (high << 4) | low;
        }

        return result / byte.MaxValue;

        int HexCharToInt(char c)
        {
            return c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'A' and <= 'F' => c - 'A' + 10,
                >= 'a' and <= 'f' => c - 'a' + 10,
                _ => throw new ArgumentException("Invalid hex char"),
            };
        }
    }

    /// <summary>
    /// Converts a hex color code to linear [0, 1].
    /// </summary>
    public static Vector4 HexToLinear(string hex)
    {
        return SrgbToLinear(HexToSrgb(hex));
    }

    #endregion

    #region Color struct

    /// <summary>
    /// Interpolates from one value to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Color Lerp(Color from, Color to, float t, ColorType mixingSpace = ColorType.Srgb)
    {
        from = from.As(mixingSpace);
        to = to.As(mixingSpace);

        return new Color(Lerp(from.Value, to.Value, t), mixingSpace);
    }

    /// <summary>
    /// Interpolates from one value to another by <see cref="t"/>.
    /// </summary>
    public static Color LerpUnclamped(Color from, Color to, float t, ColorType mixingSpace = ColorType.Srgb)
    {
        from = from.As(mixingSpace);
        to = to.As(mixingSpace);

        return new Color(LerpUnclamped(from.Value, to.Value, t), mixingSpace);
    }

    #endregion
}

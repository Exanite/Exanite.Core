using System;
using System.Numerics;
using Exanite.Core.Utilities;
using DrawingColor = System.Drawing.Color;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose color representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
/// <remarks>
/// Consider using one of the storage types if you want efficient storage:
/// <see cref="LinearColor4"/>,
/// <see cref="LinearColor3"/>,
/// <see cref="SrgbColor4"/>,
/// <see cref="SrgbColor3"/>,
/// <see cref="HslColor4"/>,
/// <see cref="HslColor3"/>
/// </remarks>
public partial record struct Color
{
    private Vector4 color;
    private ColorType type;

    public ColorType Type
    {
        readonly get => type;
        set => type = value;
    }

    public Vector4 Value
    {
        readonly get => color;
        set => color = value;
    }

    public float X
    {
        readonly get => color.X;
        set => color.X = value;
    }

    public float Y
    {
        readonly get => color.Y;
        set => color.Y = value;
    }

    public float Z
    {
        readonly get => color.Z;
        set => color.Z = value;
    }

    public float W
    {
        readonly get => color.W;
        set => color.W = value;
    }

    public Color(Vector4 color, ColorType type)
    {
        this.color = color;
        this.type = type;
    }

    // Srgb

    public static Color FromBytesSrgb(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        return new Color(new Vector4(r, g, b, a) / byte.MaxValue, ColorType.Srgb);
    }

    // System.Drawing.Color

    public static implicit operator Color(DrawingColor color)
    {
        return FromDrawingColor(color);
    }

    public static implicit operator DrawingColor(Color color)
    {
        return color.ToDrawingColor();
    }

    public static Color FromDrawingColor(DrawingColor color)
    {
        var value = new Vector4(color.R, color.G, color.B, color.A) / byte.MaxValue;
        return new Color(value, ColorType.Srgb);
    }

    public readonly DrawingColor ToDrawingColor()
    {
        var value = Srgb.Value * byte.MaxValue;
        value = new Vector4(float.Round(value.X), float.Round(value.Y), float.Round(value.Z), float.Round(value.W));

        return DrawingColor.FromArgb((byte)value.W, (byte)value.X, (byte)value.Y, (byte)value.Z);
    }

    // Hex

    public static Color FromHex(string hex)
    {
        return new Color(M.HexToSrgb(hex), ColorType.Srgb);
    }

    public readonly string ToHex(bool includeAlpha = true)
    {
        var drawingColor = ToDrawingColor();

        return includeAlpha
            ? $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}{drawingColor.A:X2}"
            : $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}";
    }

    // Ansi

    public readonly string ToAnsi()
    {
        var drawingColor = AsPremultiplied().ToDrawingColor();
        var value = (drawingColor.R << 16) | (drawingColor.G << 8) | (drawingColor.B << 0);

        return AnsiUtility.HexColorToAnsi(value);
    }

    public readonly string ToAnsiForeground()
    {
        return AnsiUtility.AnsiForeground(ToAnsi());
    }

    public readonly string ToAnsiBackground()
    {
        return AnsiUtility.AnsiBackground(ToAnsi());
    }

    // Conversions

    public readonly Color As(ColorType type)
    {
        return Convert(this, type);
    }

    private static Color Convert(Color color, ColorType targetType)
    {
        if (color.Type == targetType)
        {
            return color;
        }

        // Implementation note:
        // All conversions go through linear to keep things simple, but does lead to some inefficiencies

        // Convert to linear first
        var value = color.Value;
        switch (color.Type)
        {
            case ColorType.Linear:
            {
                break;
            }
            case ColorType.Srgb:
            {
                value = M.SrgbToLinear(value);
                break;
            }
            case ColorType.Hsl:
            {
                // Based on https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
                var h = M.Wrap(value.X, 0, 360); // [0, 360)
                var s = value.Y; // [0, 1]
                var l = value.Z; // [0, 1]
                var a = value.W; // [0, 1]

                var section = h / 60;
                var c = (1 - M.Abs(2 * l - 1)) * s;
                var x = c * (1 - M.Abs((section % 2) - 1));
                var m = l - (c / 2);

                var srgb = (int)section switch
                {
                    0 => new Vector4(c, x, 0, a),
                    1 => new Vector4(x, c, 0, a),
                    2 => new Vector4(0, c, x, a),
                    3 => new Vector4(0, x, c, a),
                    4 => new Vector4(x, 0, c, a),
                    5 => new Vector4(c, 0, x, a),
                    _ => throw new InvalidOperationException("Section value is out of range. This indicates an implementation error"),
                };

                srgb += new Vector4(m, m, m, 0);

                // This is inefficient if the requested type is Srgb
                value = M.SrgbToLinear(srgb);

                break;
            }
            default:
            {
                ExceptionUtility.ThrowNotSupported(color.Type);
                break;
            }
        }

        // Convert to output type
        switch (targetType)
        {
            case ColorType.Linear:
            {
                break;
            }
            case ColorType.Srgb:
            {
                value = M.LinearToSrgb(value);
                break;
            }
            case ColorType.Hsl:
            {
                value = M.LinearToSrgb(value);

                var r = value.X;
                var g = value.Y;
                var b = value.Z;
                var a = value.W;

                var xMax = M.Max(M.Max(r, g), b);
                var xMin = M.Min(M.Min(r, g), b);

                var c = xMax - xMin;
                var l = (xMax + xMin) / 2;

                var h = 0f;
                if (c == 0)
                {
                    h = 0;
                }
                else if (xMax.Equals(r))
                {
                    h = 60 * (((g - b) / c) % 6);
                }
                else if (xMax.Equals(g))
                {
                    h = 60 * (((b - r) / c) + 2);
                }
                else if (xMax.Equals(b))
                {
                    h = 60 * (((r - g) / c) + 4);
                }

                // The above can return negative values
                // Eg: For linear input, (1, 0, 1)
                h = M.Wrap(h, 0, 360);

                var s = 0f;
                if (l > 0 && l < 1)
                {
                    s = (xMax - l) / M.Min(l, 1 - l);
                }

                value = new Vector4(h, s, l, a);

                break;
            }
            default:
            {
                ExceptionUtility.ThrowNotSupported(targetType);
                break;
            }
        }

        return new Color(value, targetType);
    }

    public readonly Color WithTypeOverride(ColorType type)
    {
        return new Color(Value, type);
    }

    // Comparisons

    public readonly bool ApproximatelyEquals(Color other, ColorType colorType = ColorType.Linear, float tolerance = 0.000001f)
    {
        return M.ApproximatelyEquals(As(colorType).Value, other.As(colorType).Value, tolerance);
    }

    // Operations

    public readonly Color AsPremultiplied()
    {
        return M.Premultiply(this);
    }

    public readonly Color WithAlpha(float a)
    {
        return this with
        {
            W = a,
        };
    }

    public readonly override string ToString()
    {
        return $"{Value} ({Type})";
    }
}

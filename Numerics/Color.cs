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
/// <see cref="SrgbColor4"/>
/// </remarks>
public struct Color : IEquatable<Color>
{
    public static Color White => FromSrgb(1, 1, 1);
    public static Color Black => FromSrgb(0, 0, 0);
    public static Color Red => FromSrgb(1, 0, 0);
    public static Color Green => FromSrgb(0, 1, 0);
    public static Color Blue => FromSrgb(0, 0, 1);
    public static Color Yellow => FromSrgb(1, 1, 0);
    public static Color Cyan => FromSrgb(0, 1, 1);
    public static Color Magenta => FromSrgb(1, 0, 1);

    public static Color TransparentWhite => FromSrgb(1, 1, 1, 0);
    public static Color TransparentBlack => FromSrgb(0, 0, 0, 0);

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

    public readonly Color Srgb => As(ColorType.Srgb);

    public static implicit operator Color(SrgbColor4 color)
    {
        return new Color(color.Value, ColorType.Srgb);
    }

    public static implicit operator SrgbColor4(Color color)
    {
        return new SrgbColor4(color.As(ColorType.Srgb).Value);
    }

    public static Color FromSrgb(Vector3 value)
    {
        return new Color(value.Xyz1(), ColorType.Srgb);
    }

    public static Color FromSrgb(Vector4 value)
    {
        return new Color(value, ColorType.Srgb);
    }

    public static Color FromSrgb(float r, float g, float b, float a = 1)
    {
        return new Color(new Vector4(r, g, b, a), ColorType.Srgb);
    }

    public static Color FromBytesSrgb(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        return new Color(new Vector4(r, g, b, a) / byte.MaxValue, ColorType.Srgb);
    }

    // Linear

    public readonly Color Linear => As(ColorType.Linear);

    public static implicit operator Color(LinearColor4 color)
    {
        return new Color(color.Value, ColorType.Linear);
    }

    public static implicit operator LinearColor4(Color color)
    {
        return new LinearColor4(color.As(ColorType.Linear).Value);
    }

    public static Color FromLinear(Vector3 value)
    {
        return new Color(value.Xyz1(), ColorType.Linear);
    }

    public static Color FromLinear(Vector4 value)
    {
        return new Color(value, ColorType.Linear);
    }

    public static Color FromLinear(float r, float g, float b, float a = 1)
    {
        return new Color(new Vector4(r, g, b, a), ColorType.Linear);
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
        return DrawingColor.FromArgb((byte)value.W, (byte)value.X, (byte)value.Y, (byte)value.Z);
    }

    // Hex

    public static Color FromHex(string hex)
    {
        return new Color(M.HexToSrgb(hex), ColorType.Srgb);
    }

    public readonly string ToHex()
    {
        var drawingColor = ToDrawingColor();
        return $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}{drawingColor.A:X2}";
    }

    // Ansi

    public readonly string ToAnsi()
    {
        var drawingColor = ToDrawingColor();
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
        if (Type == type)
        {
            return this;
        }

        // Convert to linear first
        var value = Value;
        switch (Type)
        {
            case ColorType.Srgb:
            {
                value = M.SrgbToLinear(value);
                break;
            }
            case ColorType.Linear:
            {
                break;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(Type);
            }
        }

        // Convert to output type
        switch (type)
        {
            case ColorType.Srgb:
            {
                value = M.LinearToSrgb(value);
                break;
            }
            case ColorType.Linear:
            {
                break;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(Type);
            }
        }

        return new Color(value, type);
    }

    public readonly Color WithTypeOverride(ColorType type)
    {
        return new Color(Value, type);
    }

    // Comparisons

    public static bool operator ==(Color a, Color b)
    {
        b = b.As(a.Type);
        return M.IsApproximatelyEqual(a.Value, b.Value);
    }

    public static bool operator !=(Color a, Color b)
    {
        b = b.As(a.Type);
        return !M.IsApproximatelyEqual(a.Value, b.Value);
    }

    public readonly bool Equals(Color other)
    {
        return Value.Equals(other.Value)
               && Type == other.Type;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Angle other && Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(Value, Type);
    }

    // Operations

    public readonly override string ToString()
    {
        return $"{Value} ({Type})";
    }

    // Predefined palettes
    // These palettes are mainly for debugging or placeholder purposes

    /// <summary>
    /// Color palette based on Minecraft 1.12 dye colors.
    /// See https://minecraft.fandom.com/wiki/Dye
    /// </summary>
    public static class Dyes
    {
        public static Color White => FromHex("#F9FFFE");
        public static Color LightGray => FromHex("#9D9D97");
        public static Color Gray => FromHex("#474F52");
        public static Color Black => FromHex("#1D1D21");
        public static Color Brown => FromHex("#835432");
        public static Color Red => FromHex("#B02E26");
        public static Color Orange => FromHex("#F9801D");
        public static Color Yellow => FromHex("#FED83D");
        public static Color Lime => FromHex("#80C71F");
        public static Color Green => FromHex("#5E7C16");
        public static Color Cyan => FromHex("#169C9C");
        public static Color LightBlue => FromHex("#3AB3DA");
        public static Color Blue => FromHex("#3C44AA");
        public static Color Purple => FromHex("#8932B8");
        public static Color Magenta => FromHex("#C74EBD");
        public static Color Pink => FromHex("#F38BAA");
    }
}

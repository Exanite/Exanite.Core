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
/// <see cref="LinearColor"/>,
/// <see cref="SrgbColor"/>
/// </remarks>
public struct Color
{
    public Color White => FromSrgb(1, 1, 1);
    public Color Black => FromSrgb(1, 1, 1);
    public Color Red => FromSrgb(1, 0, 0);
    public Color Green => FromSrgb(0, 1, 0);
    public Color Blue => FromSrgb(0, 0, 1);
    public Color Yellow => FromSrgb(1, 1, 0);
    public Color Cyan => FromSrgb(0, 1, 1);
    public Color Magenta => FromSrgb(1, 0, 1);

    private Vector4 color;
    private ColorType type;

    public ColorType Type
    {
        get => type;
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

    public Color Srgb => As(ColorType.Srgb);

    public static implicit operator Color(SrgbColor color)
    {
        return new Color(color.Value, ColorType.Srgb);
    }

    public static implicit operator SrgbColor(Color color)
    {
        return new SrgbColor(color.As(ColorType.Srgb).Value);
    }

    public static Color FromSrgb(Vector4 value)
    {
        return new Color(value, ColorType.Srgb);
    }

    public static Color FromSrgb(float r, float g, float b, float a = 1)
    {
        return new Color(new Vector4(r, g, b, a), ColorType.Srgb);
    }

    public static Color FromBytesSrgb(byte r, byte g, byte b, byte a = 1)
    {
        return new Color(new Vector4(r, g, b, a) / byte.MaxValue, ColorType.Srgb);
    }

    // Linear

    public Color Linear => As(ColorType.Linear);

    public static implicit operator Color(LinearColor color)
    {
        return new Color(color.Value, ColorType.Linear);
    }

    public static implicit operator LinearColor(Color color)
    {
        return new LinearColor(color.As(ColorType.Linear).Value);
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

    public DrawingColor ToDrawingColor()
    {
        var value = Srgb.Value * byte.MaxValue;
        return DrawingColor.FromArgb((byte)value.W, (byte)value.X, (byte)value.Y, (byte)value.Z);
    }

    // Hex

    public static Color FromHex(string hex)
    {
        return new Color(M.HexToSrgb(hex), ColorType.Srgb);
    }

    public string ToHex()
    {
        var drawingColor = ToDrawingColor();
        return $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}{drawingColor.A:X2}";
    }

    // Ansi

    public string ToAnsi()
    {
        var drawingColor = ToDrawingColor();
        var value = (drawingColor.R << 16) | (drawingColor.G << 8) | (drawingColor.B << 0);

        return AnsiUtility.HexColorToAnsi(value);
    }

    public string ToAnsiForeground()
    {
        return AnsiUtility.AnsiForeground(ToAnsi());
    }

    public string ToAnsiBackground()
    {
        return AnsiUtility.AnsiBackground(ToAnsi());
    }

    // Conversions

    public Color As(ColorType type)
    {
        if (Type == type)
        {
            return this;
        }

        // Convert to linear first
        switch (Type)
        {
            case ColorType.Srgb:
            {
                Value = M.SrgbToLinear(Value);
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
                Value = M.LinearToSrgb(Value);
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

        return new Color(Value, type);
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

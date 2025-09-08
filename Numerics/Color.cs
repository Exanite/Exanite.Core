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
}

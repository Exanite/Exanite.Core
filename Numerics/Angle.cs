using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Specifies the unit used to represent angles.
/// </summary>
public enum AngleType
{
    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 360], but may be wrapped when exceeding this range.
    /// </summary>
    Degrees = 0,

    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 2pi], but may be wrapped when exceeding this range.
    /// </summary>
    Radians,
}

public struct Angle
{
    private float value;
    private AngleType type;

    public AngleType Type
    {
        get => type;
        set => type = value;
    }


    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Radians
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Degrees
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

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

    public static implicit operator Color(SrgbColor color)
    {
        return new Color(color.Value, ColorType.Srgb);
    }

    public static implicit operator Color(LinearColor color)
    {
        return new Color(color.Value, ColorType.Linear);
    }

    public static implicit operator SrgbColor(Color color)
    {
        return new SrgbColor(color.As(ColorType.Srgb).Value);
    }

    public static implicit operator LinearColor(Color color)
    {
        return new LinearColor(color.As(ColorType.Linear).Value);
    }

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

[StructLayout(LayoutKind.Sequential)]
public struct LinearColor
{
    private Vector4 color;

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

    public LinearColor(Vector4 color)
    {
        this.color = color;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SrgbColor
{
    private Vector4 color;

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

    public SrgbColor(Vector4 color)
    {
        this.color = color;
    }
}

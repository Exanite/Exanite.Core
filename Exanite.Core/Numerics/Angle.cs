using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose angle representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
/// <remarks>
/// Consider using one of the storage types if you want efficient storage:
/// <see cref="Radians"/>,
/// <see cref="Degrees"/>
/// </remarks>
public record struct Angle
{
    public static Angle Zero => FromRadians(0);
    public static Angle Pi => FromRadians(float.Pi);

    private float angle;
    private AngleType type;

    public AngleType Type
    {
        readonly get => type;
        set => type = value;
    }

    public float Value
    {
        readonly get => angle;
        set => angle = value;
    }

    public Angle(float angle, AngleType type)
    {
        this.angle = angle;
        this.type = type;
    }

    // Degrees

    public readonly Angle Degrees => As(AngleType.Degrees);

    public static implicit operator Angle(Degrees angle)
    {
        return new Angle(angle.Value, AngleType.Degrees);
    }

    public static implicit operator Degrees(Angle angle)
    {
        return new Degrees(angle.Degrees.Value);
    }

    public static Angle FromDegrees(float value)
    {
        return new Angle(value, AngleType.Degrees);
    }

    // Radians

    public readonly Angle Radians => As(AngleType.Radians);

    public static implicit operator Angle(Radians angle)
    {
        return new Angle(angle.Value, AngleType.Radians);
    }

    public static implicit operator Radians(Angle angle)
    {
        return new Radians(angle.Radians.Value);
    }

    public static Angle FromRadians(float value)
    {
        return new Angle(value, AngleType.Radians);
    }

    // Conversions

    public readonly Angle As(AngleType type)
    {
        if (Type == type)
        {
            return this;
        }

        // Convert to radians first
        var value = Value;
        switch (Type)
        {
            case AngleType.Degrees:
            {
                value = M.Deg2Rad(value);
                break;
            }
            case AngleType.Radians:
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
            case AngleType.Degrees:
            {
                value = M.Rad2Deg(value);
                break;
            }
            case AngleType.Radians:
            {
                break;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(Type);
            }
        }

        return new Angle(value, type);
    }

    public readonly Angle WithTypeOverride(AngleType type)
    {
        return new Angle(Value, type);
    }

    // Comparisons

    public readonly bool ApproximatelyEquals(Angle other, AngleType angleType = AngleType.Radians, float tolerance = 0.000001f)
    {
        return M.ApproximatelyEquals(As(angleType).Value, other.As(angleType).Value, tolerance);
    }

    // Operators

    public static Angle operator +(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return new Angle(a.Value + b.Value, a.Type);
    }

    public static Angle operator -(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return new Angle(a.Value - b.Value, a.Type);
    }

    public static Angle operator -(Angle a)
    {
        return new Angle(-a.Value, a.Type);
    }

    public static Angle operator *(Angle a, float scalar)
    {
        return new Angle(a.Value * scalar, a.Type);
    }

    public static Angle operator *(float scalar, Angle a)
    {
        return new Angle(a.Value * scalar, a.Type);
    }

    public static Angle operator /(Angle a, float scalar)
    {
        return new Angle(a.Value / scalar, a.Type);
    }

    // Operations

    public readonly override string ToString()
    {
        return $"{Value} {Type}";
    }
}

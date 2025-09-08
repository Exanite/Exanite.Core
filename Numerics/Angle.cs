using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose angle representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
public struct Angle : IEquatable<Angle>, IComparable<Angle>
{
    public static Angle Zero => FromRadians(0);
    public static Angle Pi => FromRadians(float.Pi);

    private float angle;
    private AngleType type;

    public AngleType Type
    {
        get => type;
        set => type = value;
    }

    public float Value
    {
        get => angle;
        set => angle = value;
    }

    public Angle(float angle, AngleType type)
    {
        this.angle = angle;
        this.type = type;
    }

    // Degrees

    public Angle Degrees => As(AngleType.Degrees);

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

    public Angle Radians => As(AngleType.Radians);

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

    public Angle As(AngleType type)
    {
        if (Type == type)
        {
            return this;
        }

        // Convert to radians first
        switch (Type)
        {
            case AngleType.Degrees:
            {
                Value = M.Deg2Rad(Value);
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
                Value = M.Rad2Deg(Value);
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

        return new Angle(Value, type);
    }

    // Comparisons

    public static bool operator ==(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return M.IsApproximatelyEqual(a.Value, b.Value);
    }

    public static bool operator !=(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return !M.IsApproximatelyEqual(a.Value, b.Value);
    }

    public static bool operator <(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return a.Value < b.Value;
    }

    public static bool operator >(Angle a, Angle b)
    {
        b = b.As(a.Type);
        return a.Value > b.Value;
    }

    public bool Equals(Angle other)
    {
        return this == other;
    }

    public int CompareTo(Angle other)
    {
        other = other.As(Type);
        
        if (M.IsApproximatelyEqual(Value, other.Value))
        {
            return 0;
        }

        return Value < other.Value ? -1 : 1;
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Radians.Value.GetHashCode();
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
}

using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose angle representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
/// <remarks>
/// Consider using one of the storage types if you want consistent and efficient storage:
/// <see cref="Radians"/>,
/// <see cref="Degrees"/>
/// </remarks>
public record struct Angle
{
    public static Angle Zero => FromRadians(0);
    public static Angle Pi => FromRadians(float.Pi);

    private float radians;

    private Angle(float radians)
    {
        this.radians = radians;
    }

    // Degrees

    public readonly Degrees Degrees => this;

    public static Angle FromDegrees(float value)
    {
        return From(value, AngleType.Degrees);
    }

    // Radians

    public readonly Radians Radians => this;

    public static Angle FromRadians(float value)
    {
        return From(value, AngleType.Radians);
    }

    // Conversions

    public static Angle From(float value, AngleType type)
    {
        switch (type)
        {
            case AngleType.Degrees:
            {
                return new Angle(M.Deg2Rad(value));
            }
            case AngleType.Radians:
            {
                return new Angle(value);
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(type);
            }
        }
    }

    public readonly float To(AngleType type)
    {
        switch (type)
        {
            case AngleType.Degrees:
            {
                return M.Rad2Deg(radians);
            }
            case AngleType.Radians:
            {
                return radians;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(type);
            }
        }
    }

    // Comparisons

    public readonly bool ApproximatelyEquals(Angle other, AngleType angleType = AngleType.Radians, float tolerance = 0.000001f)
    {
        return M.ApproximatelyEquals(To(angleType), other.To(angleType), tolerance);
    }

    // Operators

    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle(a.radians + b.radians);
    }

    public static Angle operator -(Angle a, Angle b)
    {
        return new Angle(a.radians - b.radians);
    }

    public static Angle operator -(Angle a)
    {
        return new Angle(-a.radians);
    }

    public static Angle operator *(Angle a, float scalar)
    {
        return new Angle(a.radians * scalar);
    }

    public static Angle operator *(float scalar, Angle a)
    {
        return new Angle(a.radians * scalar);
    }

    public static Angle operator /(Angle a, float scalar)
    {
        return new Angle(a.radians / scalar);
    }

    // Operations

    public readonly override string ToString()
    {
        return $"{radians} radians";
    }
}

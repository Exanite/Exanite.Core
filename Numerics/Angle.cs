using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose angle representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
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

    public Angle(float value, AngleType type)
    {
        this.value = value;
        this.type = type;
    }

    // Degrees

    public static implicit operator Angle(Degrees angle)
    {
        return new Angle(angle.Value, AngleType.Degrees);
    }

    public static implicit operator Degrees(Angle angle)
    {
        return new Degrees(angle.As(AngleType.Degrees).Value);
    }

    public static Angle FromDegrees(float value)
    {
        return new Angle(value, AngleType.Degrees);
    }

    // Radians

    public static implicit operator Angle(Radians angle)
    {
        return new Angle(angle.Value, AngleType.Radians);
    }

    public static implicit operator Radians(Angle angle)
    {
        return new Radians(angle.As(AngleType.Radians).Value);
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
}

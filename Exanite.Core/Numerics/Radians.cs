using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in radians format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in radians.
/// <br/>
/// See <see cref="Numerics.Angle"/> for a general angle representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Radians
{
    public float Value;
    public Angle Angle => Angle.FromRadians(Value);

    public Radians(float value)
    {
        Value = value;
    }

    public static implicit operator Angle(Radians angle)
    {
        return Angle.From(angle.Value, AngleType.Radians);
    }

    public static implicit operator Radians(Angle angle)
    {
        return new Radians(angle.To(AngleType.Radians));
    }

    public static implicit operator Radians(float angle)
    {
        return new Radians(angle);
    }

    public static implicit operator float(Radians angle)
    {
        return angle.Value;
    }

    public readonly override string ToString()
    {
        return $"{Value} radians";
    }
}

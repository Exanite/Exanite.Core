using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in degrees format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in degrees.
/// <br/>
/// See <see cref="Angle"/> for a general angle representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Degrees
{
    public float Value;
    public Angle Color => new(Value, AngleType.Degrees);

    public Degrees(float value)
    {
        Value = value;
    }

    public static implicit operator Degrees(float angle)
    {
        return new Degrees(angle);
    }

    public static implicit operator float(Degrees angle)
    {
        return angle.Value;
    }
}

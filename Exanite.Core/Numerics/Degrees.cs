using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in degrees format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in degrees.
/// <br/>
/// See <see cref="Numerics.Angle"/> for a general angle representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Degrees
{
    public float Value;
    public readonly Angle Angle => this;

    public Degrees(float value)
    {
        Value = value;
    }

    public static implicit operator Angle(Degrees angle)
    {
        return Angle.FromDegrees(angle.Value);
    }

    public static implicit operator Degrees(Angle angle)
    {
        return new Degrees(angle.Degrees.Value);
    }

    public static implicit operator Degrees(float angle)
    {
        return new Degrees(angle);
    }

    public static implicit operator float(Degrees angle)
    {
        return angle.Value;
    }

    public readonly override string ToString()
    {
        return Angle.ToString();
    }
}

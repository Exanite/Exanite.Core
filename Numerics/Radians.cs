using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in radians format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in radians.
/// <br/>
/// See <see cref="Angle"/> for a general angle representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Radians
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }

    public Radians(float value)
    {
        this.value = value;
    }

    public static implicit operator Radians(float angle)
    {
        return new Radians(angle);
    }

    public static implicit operator float(Radians angle)
    {
        return angle.Value;
    }
}

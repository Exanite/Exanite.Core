using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in sRGB format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in sRGB.
/// <br/>
/// See <see cref="Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct SrgbColor3
{
    public Vector3 Value;
    public Color Color => Color.FromSrgb(Value);

    public SrgbColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator SrgbColor3(Vector3 angle)
    {
        return new SrgbColor3(angle);
    }

    public static implicit operator Vector3(SrgbColor3 angle)
    {
        return angle.Value;
    }
}

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
public record struct SrgbColor4
{
    public Vector4 Value;
    public Color Color => Color.FromSrgb(Value);

    public SrgbColor4(Vector4 value)
    {
        Value = value;
    }

    public static implicit operator SrgbColor4(Vector4 angle)
    {
        return new SrgbColor4(angle);
    }

    public static implicit operator Vector4(SrgbColor4 angle)
    {
        return angle.Value;
    }
}

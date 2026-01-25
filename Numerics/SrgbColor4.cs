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
    private Vector4 color;

    public Vector4 Value
    {
        readonly get => color;
        set => color = value;
    }

    public Color Color => Color.FromSrgb(color);

    public SrgbColor4(Vector4 color)
    {
        this.color = color;
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

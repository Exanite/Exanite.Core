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
    private Vector3 color;

    public Vector3 Value
    {
        readonly get => color;
        set => color = value;
    }

    public Color Color => Color.FromSrgb(color);

    public SrgbColor3(Vector3 color)
    {
        this.color = color;
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

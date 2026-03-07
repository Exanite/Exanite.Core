using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing a color in sRGB format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in sRGB.
/// <br/>
/// See <see cref="Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct SrgbColor3
{
    public Vector3 Value;
    public readonly Color Color => this;

    public SrgbColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator Color(SrgbColor3 color)
    {
        return Color.FromSrgb(color.Value);
    }

    public static implicit operator SrgbColor3(Color color)
    {
        return new SrgbColor3(color.Srgb.Value.Xyz());
    }

    public static implicit operator SrgbColor3(Vector3 color)
    {
        return new SrgbColor3(color);
    }

    public static implicit operator Vector3(SrgbColor3 color)
    {
        return color.Value;
    }

    public readonly override string ToString()
    {
        return Color.ToString();
    }
}

using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing a color in sRGB format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in sRGB.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct SrgbColor4
{
    public Vector4 Value;
    public readonly Color Color => this;

    public SrgbColor4(Vector4 value)
    {
        Value = value;
    }

    public static implicit operator Color(SrgbColor4 color)
    {
        return Color.FromSrgb(color.Value);
    }

    public static implicit operator SrgbColor4(Color color)
    {
        return new SrgbColor4(color.Srgb.Value);
    }

    public static implicit operator SrgbColor4(Vector4 color)
    {
        return new SrgbColor4(color);
    }

    public static implicit operator Vector4(SrgbColor4 color)
    {
        return color.Value;
    }

    public readonly override string ToString()
    {
        return Color.ToString();
    }
}

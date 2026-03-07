using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing a color in HSL format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in HSL.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct HslColor3
{
    public Vector3 Value;
    public readonly Color Color => this;

    public HslColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator Color(HslColor3 color)
    {
        return Color.FromHsl(color.Value);
    }

    public static implicit operator HslColor3(Color color)
    {
        return new HslColor3(color.Hsl.Value.Xyz());
    }

    public static implicit operator HslColor3(Vector3 color)
    {
        return new HslColor3(color);
    }

    public static implicit operator Vector3(HslColor3 color)
    {
        return color.Value;
    }

    public readonly override string ToString()
    {
        return Color.ToString();
    }
}

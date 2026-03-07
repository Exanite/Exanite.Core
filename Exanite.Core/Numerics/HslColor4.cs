using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in HSL format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in HSL.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct HslColor4
{
    public Vector4 Value;
    public Color Color => Color.FromHsl(Value);

    public HslColor4(Vector4 value)
    {
        Value = value;
    }

    public static implicit operator Color(HslColor4 color)
    {
        return Color.From(color.Value, ColorType.Hsl);
    }

    public static implicit operator HslColor4(Color color)
    {
        return new HslColor4(color.To(ColorType.Hsl));
    }

    public static implicit operator HslColor4(Vector4 angle)
    {
        return new HslColor4(angle);
    }

    public static implicit operator Vector4(HslColor4 angle)
    {
        return angle.Value;
    }

    public readonly override string ToString()
    {
        return $"{Value} (HSL)";
    }
}

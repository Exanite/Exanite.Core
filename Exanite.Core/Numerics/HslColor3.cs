using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in HSL format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in HSL.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct HslColor3
{
    public Vector3 Value;
    public Color Color => Color.FromHsl(Value);

    public HslColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator Color(HslColor3 color)
    {
        return Color.From(color.Value.Xyz1(), ColorType.Hsl);
    }

    public static implicit operator HslColor3(Color color)
    {
        return new HslColor3(color.To(ColorType.Hsl).Xyz());
    }

    public readonly override string ToString()
    {
        return $"{Value} (HSL)";
    }
}

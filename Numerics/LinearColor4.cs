using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in linear format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in linear.
/// <br/>
/// See <see cref="Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LinearColor4
{
    public Vector4 Value;
    public Color Color => Color.FromLinear(Value);

    public LinearColor4(Vector4 value)
    {
        Value = value;
    }

    public static implicit operator LinearColor4(Vector4 angle)
    {
        return new LinearColor4(angle);
    }

    public static implicit operator Vector4(LinearColor4 angle)
    {
        return angle.Value;
    }
}

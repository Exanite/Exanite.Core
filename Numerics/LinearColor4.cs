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
public struct LinearColor4
{
    private Vector4 color;

    public Vector4 Value
    {
        readonly get => color;
        set => color = value;
    }

    public Color Color => Color.FromLinear(color);

    public LinearColor4(Vector4 color)
    {
        this.color = color;
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

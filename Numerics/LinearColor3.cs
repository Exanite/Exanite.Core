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
public struct LinearColor3
{
    private Vector3 color;

    public Vector3 Value
    {
        readonly get => color;
        set => color = value;
    }

    public Color Color => Color.FromLinear(color);

    public LinearColor3(Vector3 color)
    {
        this.color = color;
    }

    public static implicit operator LinearColor3(Vector3 angle)
    {
        return new LinearColor3(angle);
    }

    public static implicit operator Vector3(LinearColor3 angle)
    {
        return angle.Value;
    }
}

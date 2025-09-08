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
public struct LinearColor
{
    private Vector4 color;

    public Vector4 Value
    {
        readonly get => color;
        set => color = value;
    }

    public float X
    {
        readonly get => color.X;
        set => color.X = value;
    }

    public float Y
    {
        readonly get => color.Y;
        set => color.Y = value;
    }

    public float Z
    {
        readonly get => color.Z;
        set => color.Z = value;
    }

    public float W
    {
        readonly get => color.W;
        set => color.W = value;
    }

    public LinearColor(Vector4 color)
    {
        this.color = color;
    }

    // Color

    public Color Color => new(color, ColorType.Linear);

    // Vector4

    public static implicit operator LinearColor(Vector4 angle)
    {
        return new LinearColor(angle);
    }

    public static implicit operator Vector4(LinearColor angle)
    {
        return angle.Value;
    }
}

using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

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
}
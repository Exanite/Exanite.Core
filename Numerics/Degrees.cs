using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Degrees
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }

    public Degrees(float value)
    {
        this.value = value;
    }
}
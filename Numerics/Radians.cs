using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Radians
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }

    public Radians(float value)
    {
        this.value = value;
    }
}
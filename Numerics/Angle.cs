using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Specifies the unit used to represent angles.
/// </summary>
public enum AngleType
{
    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 360], but may be wrapped when exceeding this range.
    /// </summary>
    Degrees = 0,

    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 2pi], but may be wrapped when exceeding this range.
    /// </summary>
    Radians,
}

public struct Angle
{
    private float value;
    private AngleType type;

    public AngleType Type
    {
        get => type;
        set => type = value;
    }


    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Radians
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Degrees
{
    private float value;

    public float Value
    {
        get => value;
        set => this.value = value;
    }
}

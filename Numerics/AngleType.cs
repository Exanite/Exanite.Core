namespace Exanite.Core.Numerics;

/// <summary>
/// Specifies the unit used to represent angles.
/// </summary>
public enum AngleType
{
    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 2pi], but may be wrapped when exceeding this range.
    /// </summary>
    Radians = 0,

    /// <summary>
    /// Angle measured in degrees.
    /// Ranges from [0, 360], but may be wrapped when exceeding this range.
    /// </summary>
    Degrees,
}
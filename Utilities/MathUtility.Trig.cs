using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Trigonometry

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    public static float Rad2Deg<T>(T radians) where T : INumber<T>
    {
        return float.CreateChecked(radians) * (180 / float.Pi);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    public static float Deg2Rad<T>(T degrees) where T : INumber<T>
    {
        return float.CreateChecked(degrees) * (float.Pi / 180);
    }

    /// <summary>
    /// Gets the smallest signed difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T AngleDifferenceRadians<T>(T current, T target) where T : IFloatingPoint<T>
    {
        var delta = Wrap(target - current, T.Zero, T.Pi + T.Pi);
        if (delta > T.Pi)
        {
            delta -= T.Pi + T.Pi;
        }

        return delta;
    }

    /// <summary>
    /// Gets the smallest signed difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T AngleDifferenceDegrees<T>(T current, T target) where T : INumber<T>
    {
        var delta = Wrap(target - current, T.Zero, T.CreateTruncating(360));
        if (delta > T.CreateTruncating(180))
        {
            delta -= T.CreateTruncating(360);
        }

        return delta;
    }

    /// <summary>
    /// Gets the smallest positive difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T AngleBetweenRadians<T>(T current, T target) where T : IFloatingPoint<T>
    {
        return AngleDifferenceRadians(current, target);
    }

    /// <summary>
    /// Gets the smallest positive difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T AngleBetweenDegrees<T>(T current, T target) where T : INumber<T>
    {
        return AngleDifferenceDegrees(current, target);
    }

    #endregion

    #region Angle struct

    /// <summary>
    /// Returns the absolute value of the provided angle.
    /// </summary>
    public static Angle Abs(this Angle value)
    {
        return Angle.FromRadians(Abs(value.Radians.Value));
    }

    /// <summary>
    /// Gets the smallest signed difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static Angle AngleDifference(Angle a, Angle b)
    {
        return Angle.FromRadians(AngleBetweenRadians(a.Radians.Value, b.Radians.Value));
    }

    /// <summary>
    /// Gets the smallest positive difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static Angle AngleBetween(Angle a, Angle b)
    {
        return Angle.FromRadians(AngleBetweenRadians(a.Radians.Value, b.Radians.Value));
    }

    /// <summary>
    /// Normalizes the angle to be in the range [0, 360] degrees or [0, 2pi] radians.
    /// </summary>
    public static Angle Normalize360(this Angle a)
    {
        return Angle.FromRadians(Wrap(a.Radians.Value, 0, 2 * float.Pi));
    }

    /// <summary>
    /// Normalizes the angle to be in the range [-180, 180] degrees or [-pi, pi] radians.
    /// </summary>
    public static Angle Normalize180(this Angle a)
    {
        return Angle.FromRadians(Wrap(a.Radians.Value, -float.Pi, float.Pi));
    }

    #endregion
}

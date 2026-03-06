using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Trigonometry

    /// <summary>
    /// Returns the sine of an angle.
    /// </summary>
    public static float Sin<T>(T radians) where T : INumber<T>
    {
        return float.Sin(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the cosine of an angle.
    /// </summary>
    public static float Cos<T>(T radians) where T : INumber<T>
    {
        return float.Cos(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the tangent of an angle.
    /// </summary>
    public static float Tan<T>(T radians) where T : INumber<T>
    {
        return float.Tan(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the arc-sine of an angle.
    /// </summary>
    public static float Asin<T>(T radians) where T : INumber<T>
    {
        return float.Asin(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the arc-cosine of an angle.
    /// </summary>
    public static float Acos<T>(T radians) where T : INumber<T>
    {
        return float.Acos(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the arc-tangent of an angle.
    /// </summary>
    public static float Atan<T>(T radians) where T : INumber<T>
    {
        return float.Atan(float.CreateChecked(radians));
    }

    /// <summary>
    /// Returns the arc-tangent of the quotient of two values.
    /// </summary>
    public static float Atan2<T>(T y, T x) where T : INumber<T>
    {
        return float.Atan2(float.CreateChecked(y), float.CreateChecked(x));
    }

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
    /// Returns the sine of an angle.
    /// </summary>
    public static float Sin(Angle angle)
    {
        return float.Sin(angle.Radians.Value);
    }

    /// <summary>
    /// Returns the cosine of an angle.
    /// </summary>
    public static float Cos(Angle angle)
    {
        return float.Cos(angle.Radians.Value);
    }

    /// <summary>
    /// Returns the tangent of an angle.
    /// </summary>
    public static float Tan(Angle angle)
    {
        return float.Tan(angle.Radians.Value);
    }

    /// <summary>
    /// Returns the arc-sine of an angle.
    /// </summary>
    public static float Asin(Angle angle)
    {
        return float.Asin(angle.Radians.Value);
    }

    /// <summary>
    /// Returns the arc-cosine of an angle.
    /// </summary>
    public static float Acos(Angle angle)
    {
        return float.Acos(angle.Radians.Value);
    }

    /// <summary>
    /// Returns the arc-tangent of an angle.
    /// </summary>
    public static float Atan(Angle angle)
    {
        return float.Atan(angle.Radians.Value);
    }

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
        return Angle.FromRadians(AngleDifferenceRadians(a.Radians.Value, b.Radians.Value));
    }

    /// <summary>
    /// Gets the smallest positive difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static Angle AngleBetween(Angle a, Angle b)
    {
        return Angle.FromRadians(AngleBetweenRadians(a.Radians.Value, b.Radians.Value));
    }

    /// <summary>
    /// Gets the smallest positive angle difference between two vectors.
    /// </summary>
    public static Angle AngleBetween(Vector2 a, Vector2 b)
    {
        return Angle.FromRadians(Acos(Vector2.Dot(a.AsNormalizedOrDefault(), b.AsNormalizedOrDefault())));
    }

    /// <summary>
    /// Gets the smallest positive angle difference between two vectors.
    /// </summary>
    public static Angle AngleBetween(Vector3 a, Vector3 b)
    {
        return Angle.FromRadians(Acos(Vector3.Dot(a.AsNormalizedOrDefault(), b.AsNormalizedOrDefault())));
    }

    /// <summary>
    /// Clamps the <see cref="value"/> to be in the range [<see cref="min"/>, <see cref="max"/>].
    /// </summary>
    public static Angle Clamp(Angle value, Angle min, Angle max)
    {
        return Angle.FromRadians(Clamp(value.Radians.Value, min.Radians.Value, max.Radians.Value));
    }

    /// <summary>
    /// Wraps the angle between min and max angles.
    /// </summary>
    public static Angle Wrap(Angle value, Angle min, Angle max)
    {
        return Angle.FromRadians(Wrap(value.Radians.Value, min.Radians.Value, max.Radians.Value));
    }

    /// <summary>
    /// Normalizes the angle to be in the range [0, 360] degrees or [0, 2pi] radians.
    /// </summary>
    public static Angle Normalize360(this Angle a)
    {
        return Wrap(a, Angle.Zero, 2 * Angle.Pi);
    }

    /// <summary>
    /// Normalizes the angle to be in the range [-180, 180] degrees or [-pi, pi] radians.
    /// </summary>
    public static Angle Normalize180(this Angle a)
    {
        return Wrap(a, -Angle.Pi, Angle.Pi);
    }

    #endregion
}

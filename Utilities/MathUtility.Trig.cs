using System.Numerics;

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
    /// Converts radians to degrees.
    /// </summary>
    public static TOut Rad2Deg<TIn, TOut>(TIn radians) where TIn : INumber<TIn> where TOut : IFloatingPoint<TOut>
    {
        return TOut.CreateChecked(radians) * (TOut.CreateTruncating(180) / TOut.Pi);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    public static float Deg2Rad<T>(T degrees) where T : INumber<T>
    {
        return float.CreateChecked(degrees) * (float.Pi / 180);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    public static TOut Deg2Rad<TIn, TOut>(TIn degrees) where TIn : INumber<TIn> where TOut : IFloatingPoint<TOut>
    {
        return TOut.CreateChecked(degrees) * (TOut.Pi / TOut.CreateChecked(180));
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
}

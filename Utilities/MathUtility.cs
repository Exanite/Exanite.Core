using System;
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities;

/// <remarks>
/// This should be named MathUtility,
/// but the methods in this class are used extremely commonly.
/// <br/>
/// This makes the methods easier to access without
/// needing to use a static import.
/// </remarks>
public static partial class M
{
    #region INumbers

    /// <summary>
    /// Returns the sign of the provided value.
    /// </summary>
    public static T Sign<T>(T a) where T : INumber<T>
    {
        return T.CreateTruncating(T.Sign(a));
    }

    /// <summary>
    /// Returns the absolute value of the provided value.
    /// </summary>
    public static T Abs<T>(T a) where T : INumber<T>
    {
        return T.Abs(a);
    }

    /// <summary>
    /// Returns the minimum of the two provided values.
    /// </summary>
    public static T Min<T>(T a, T b) where T : INumber<T>
    {
        return T.Min(a, b);
    }

    /// <summary>
    /// Returns the maximum of the two provided values.
    /// </summary>
    public static T Max<T>(T a, T b) where T : INumber<T>
    {
        return T.Max(a, b);
    }

    /// <summary>
    /// Clamps <see cref="value"/> to be in the range [<see cref="min"/>, <see cref="max"/>].
    /// </summary>
    public static T Clamp<T>(T value, T min, T max) where T : INumber<T>
    {
        return T.Clamp(value, min, max);
    }

    /// <summary>
    /// Interpolates from one value to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static T Lerp<T>(T from, T to, T t) where T : INumber<T>
    {
        t = Clamp(t, T.Zero, T.One);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one value to another by <see cref="t"/>.
    /// </summary>
    public static T LerpUnclamped<T>(T from, T to, T t) where T : INumber<T>
    {
        return from + (to - from) * t;
    }

    /// <summary>
    /// Wraps a value between min and max values.
    /// </summary>
    public static T Wrap<T>(T value, T min, T max) where T : INumber<T>
    {
        return Modulo(value - min, max - min) + min;
    }

    /// <summary>
    /// Returns the true modulo of a value when divided by a divisor.
    /// </summary>
    public static T Modulo<T>(T value, T divisor) where T : INumber<T>
    {
        return (value % divisor + divisor) % divisor;
    }

    #endregion

    #region IFloatingPoint

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static T Remap<T>(T value, T fromMin, T fromMax, T toMin, T toMax) where T : IFloatingPoint<T>
    {
        var fromRange = fromMax - fromMin;
        var toRange = toMax - toMin;

        return fromRange == T.Zero ? toMin : toRange * ((value - fromMin) / fromRange) + toMin;
    }

    /// <summary>
    /// Moves the <see cref="current"/> value towards the <see cref="target"/> value by <see cref="maxDelta"/>,
    /// ensuring that the result doesn't overshoot the <see cref="target"/> value.
    /// </summary>
    public static T MoveTowards<T>(T current, T target, T maxDelta) where T : IFloatingPoint<T>
    {
        if (Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current + Sign(target - current) * maxDelta;
    }

    /// <summary>
    /// Moves the <see cref="current"/> angle towards the <see cref="target"/> angle by <see cref="maxDelta"/>,
    /// ensuring that the result doesn't overshoot the <see cref="target"/> angle.
    /// </summary>
    public static T MoveTowardsAngleDegrees<T>(T current, T target, T maxDelta) where T : IFloatingPoint<T>
    {
        var deltaAngle = DeltaAngleDegrees(current, target);
        if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
        {
            return target;
        }

        target = current + deltaAngle;
        return MoveTowards(current, target, maxDelta);
    }

    /// <summary>
    /// Moves the <see cref="current"/> angle towards the <see cref="target"/> angle by <see cref="maxDelta"/>,
    /// ensuring that the result doesn't overshoot the <see cref="target"/> angle.
    /// </summary>
    public static T MoveTowardsAngleRadians<T>(T current, T target, T maxDelta) where T : IFloatingPoint<T>
    {
        var deltaAngle = DeltaAngleRadians(current, target);
        if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
        {
            return target;
        }

        target = current + deltaAngle;
        return MoveTowards(current, target, maxDelta);
    }

    /// <summary>
    /// Moves a value towards the target value over time while smoothing the movement.
    /// <br/>
    /// The function uses a critically damped spring model to ensure smooth transitions without overshooting.
    /// <br/>
    /// <see cref="currentVelocity"/> is updated to track the rate of change.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="target">The value to reach.</param>
    /// <param name="smoothTime">The time it takes to reach the target. Smaller values result in faster movement.</param>
    /// <param name="deltaTime">The time since the last update.</param>
    /// <param name="currentVelocity">Reference to the current velocity, modified by the function.</param>
    /// <param name="maxSpeed">Optional maximum speed. Defaults to PositiveInfinity.</param>
    /// <returns>The new value after applying smoothing.</returns>
    public static T SmoothDamp<T>(T current, T target, T smoothTime, T deltaTime, ref T currentVelocity, T maxSpeed = default) where T : struct, IFloatingPointIeee754<T>
    {
        // From https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs#L309
        // Based on Game Programming Gems 4 Chapter 1.10
        if (maxSpeed == T.Zero)
        {
            maxSpeed = T.PositiveInfinity;
        }

        smoothTime = Max(T.CreateTruncating(0.0001f), smoothTime);
        var omega = T.CreateTruncating(2) / smoothTime;

        var x = omega * deltaTime;
        var exp = T.One / (T.One + x + T.CreateTruncating(0.48f) * x * x + T.CreateTruncating(0.235f) * x * x * x);
        var change = current - target;
        var originalTo = target;

        // Clamp maximum speed
        var maxChange = maxSpeed * smoothTime;
        change = Clamp(change, -maxChange, maxChange);
        target = current - change;

        var temp = (currentVelocity + omega * change) * deltaTime;
        currentVelocity = (currentVelocity - omega * temp) * exp;
        var output = target + (change + temp) * exp;

        // Prevent overshooting
        if (originalTo - current > T.Zero == output > originalTo)
        {
            output = originalTo;
            currentVelocity = (output - originalTo) / deltaTime;
        }

        return output;
    }

    #endregion

    #region Integers

    // From https://github.com/KhronosGroup/Vulkan-Samples/blob/6f99ebecc383133be4f96c2eb8fb359743864a1d/samples/extensions/descriptor_buffer_basic/descriptor_buffer_basic.cpp#L173
    /// <summary>
    /// Returns the next multiple of alignment greater than or equal to size.
    /// </summary>
    /// <param name="size">Size in bytes.</param>
    /// <param name="alignment">Power of 2 alignment in bytes.</param>
    public static T GetAlignedSize<T>(T size, T alignment) where T : IBinaryInteger<T>
    {
        return (size + alignment - T.One) & ~(alignment - T.One);
    }

    /// <remarks>
    /// Only valid for positive integers.
    /// </remarks>
    public static T GreatestCommonDivisor<T>(T a, T b) where T : IBinaryInteger<T>
    {
        while (b != T.Zero)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    /// <remarks>
    /// Only valid for positive integers.
    /// </remarks>
    public static T LeastCommonMultiple<T>(T a, T b) where T : IBinaryInteger<T>
    {
        return a / GreatestCommonDivisor(a, b) * b;
    }

    /// <summary>
    /// Gets the nearest multiple to a value.
    /// </summary>
    /// <example>
    /// GetNearestMultiple(45, 11) will return 44.
    /// </example>
    public static T GetNearestMultiple<T>(T value, T multiple) where T : IBinaryInteger<T>
    {
        var remainder = value % multiple;
        var result = value - remainder;

        if (remainder > multiple / T.CreateTruncating(2))
        {
            result += multiple;
        }

        return result;
    }

    /// <summary>
    /// Returns the next power of 2 if <see cref="value"/> is not already a power of 2.
    /// The minimum value this method will return is 2.
    /// </summary>
    /// <example>
    /// GetNextPowerOfTwo(16) will return 16.
    /// <para/>
    /// GetNextPowerOfTwo(5) will return 8.
    /// </example>
    public static T GetNextPowerOfTwo<T>(T value) where T : IBinaryInteger<T>
    {
        if (value <= T.CreateTruncating(2))
        {
            return T.CreateTruncating(2);
        }

        return T.One << int.CreateTruncating(T.Log2(value - T.One) + T.One);
    }

    /// <summary>
    /// Returns true if the value is even.
    /// </summary>
    public static bool IsEven<T>(this T value) where T : IBinaryInteger<T>
    {
        return value % T.CreateTruncating(2) == T.Zero;
    }

    /// <summary>
    /// Returns true if the value is odd.
    /// </summary>
    public static bool IsOdd<T>(this T value) where T : IBinaryInteger<T>
    {
        return value % T.CreateTruncating(2) != T.Zero;
    }

    /// <summary>
    /// Returns true if the value is a power of 2.
    /// </summary>
    public static bool IsPowerOfTwo<T>(this T value) where T : IBinaryInteger<T>
    {
        return value > T.Zero && (value & (value - T.One)) == T.Zero;
    }

    #endregion

    #region Colors

    // sRGB-Linear conversion formulas are from: https://entropymine.com/imageworsener/srgbformula/

    /// <summary>
    /// Converts a sRGB [0, 1] color value to linear [0, 1].
    /// </summary>
    public static float SrgbToLinear(float value)
    {
        if (value <= 0.04045f)
        {
            return value / 12.92f;
        }

        return MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }

    /// <summary>
    /// Converts a linear [0, 1] color value to sRGB [0, 1].
    /// </summary>
    public static float LinearToSrgb(float value)
    {
        if (value <= 0.0031308f)
        {
            return value * 12.92f;
        }

        return MathF.Pow(value, 1 / 2.4f) * 1.055f - 0.055f;
    }

    public static Vector3 SrgbToLinear(Vector3 srgb)
    {
        return new Vector3(SrgbToLinear(srgb.X), SrgbToLinear(srgb.Y), SrgbToLinear(srgb.Z));
    }

    public static Vector3 LinearToSrgb(Vector3 srgb)
    {
        return new Vector3(LinearToSrgb(srgb.X), LinearToSrgb(srgb.Y), LinearToSrgb(srgb.Z));
    }

    public static Vector4 SrgbToLinear(Vector4 srgb)
    {
        return new Vector4(SrgbToLinear(srgb.X), SrgbToLinear(srgb.Y), SrgbToLinear(srgb.Z), srgb.W);
    }

    public static Vector4 LinearToSrgb(Vector4 srgb)
    {
        return new Vector4(LinearToSrgb(srgb.X), LinearToSrgb(srgb.Y), LinearToSrgb(srgb.Z), srgb.W);
    }

    public static Vector4 SrgbToLinear(string htmlColor)
    {
        var maxValue = (float)byte.MaxValue;
        var color = ColorTranslator.FromHtml(htmlColor);

        return SrgbToLinear(new Vector4(color.R / maxValue, color.G / maxValue, color.B / maxValue, color.A / maxValue));
    }

    #endregion

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
    /// Gets the signed difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T DeltaAngleRadians<T>(T current, T target) where T : IFloatingPoint<T>
    {
        var delta = Wrap(target - current, T.Zero, T.Pi + T.Pi);
        if (delta > T.Pi)
        {
            delta -= T.Pi + T.Pi;
        }

        return delta;
    }

    /// <summary>
    /// Gets the signed difference between two angles, while taking the wrap-around point into account.
    /// </summary>
    public static T DeltaAngleDegrees<T>(T current, T target) where T : INumber<T>
    {
        var delta = Wrap(target - current, T.Zero, T.CreateTruncating(360));
        if (delta > T.CreateTruncating(180))
        {
            delta -= T.CreateTruncating(360);
        }

        return delta;
    }

    #endregion

    #region IsApproximatelyEqual

    /// <summary>
    /// Checks if two floating point values are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual<T>(T a, T b, float tolerance = 0.000001f) where T : IFloatingPoint<T>
    {
        return IsApproximatelyEqual(a, b, T.CreateTruncating(tolerance));
    }

    /// <summary>
    /// Checks if two floating point values are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual<T>(T a, T b, T tolerance) where T : IFloatingPoint<T>
    {
        return Abs(a - b) <= tolerance;
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.000001f)
    {
        return IsApproximatelyEqual(a.X, b.X, tolerance) && IsApproximatelyEqual(a.Y, b.Y, tolerance);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual(Vector3 a, Vector3 b, float tolerance = 0.000001f)
    {
        return IsApproximatelyEqual(a.X, b.X, tolerance) && IsApproximatelyEqual(a.Y, b.Y, tolerance) && IsApproximatelyEqual(a.Z, b.Z, tolerance);
    }

    #endregion
}

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
    /// Clamps the <see cref="value"/> to be in the range [<see cref="min"/>, <see cref="max"/>].
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
    /// Wraps the value between min and max values.
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
    /// Output will be clamped in the range [toStart, toEnd].
    /// </summary>
    public static T Remap<T>(T value, T fromStart, T fromEnd, T toStart, T toEnd) where T : IFloatingPoint<T>
    {
        var t = (value - fromStart) / (fromEnd - fromStart);
        return Lerp(toStart, toEnd, t);
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static T RemapUnclamped<T>(T value, T fromStart, T fromEnd, T toStart, T toEnd) where T : IFloatingPoint<T>
    {
        var t = (value - fromStart) / (fromEnd - fromStart);
        return LerpUnclamped(toStart, toEnd, t);
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
        var deltaAngle = AngleDifferenceDegrees(current, target);
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
        var deltaAngle = AngleDifferenceRadians(current, target);
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
}

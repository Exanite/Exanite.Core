using System;
using System.Drawing;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities
{
    public static class MathUtility
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
        public static T SmoothDamp<T>(T current, T target, T smoothTime, T deltaTime, ref T currentVelocity, T maxSpeed = default) where T : IFloatingPointIeee754<T>
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
            return T.One << ((int)Math.Log2(double.CreateTruncating(value - T.One)) + 1);
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

        #region Vectors

        /// <summary>
        /// Normalizes the vector.
        /// <br/>
        /// This handles the case where the provided vector is Vector2.Zero,
        /// returning Vector2.Zero rather than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value)
        {
            return value.AsNormalizedSafe(Vector2.Zero);
        }

        /// <summary>
        /// Normalizes the vector.
        /// <br/>
        /// This handles the case where the provided vector is Vector2.Zero,
        /// returning the provided <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value, Vector2 fallback)
        {
            return value == Vector2.Zero ? fallback : Vector2.Normalize(value);
        }

        /// <summary>
        /// Normalizes the vector.
        /// <br/>
        /// This handles the case where the provided vector is Vector3.Zero,
        /// returning Vector3.Zero rather than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value)
        {
            return value.AsNormalizedSafe(Vector3.Zero);
        }

        /// <summary>
        /// Normalizes the vector.
        /// <br/>
        /// This handles the case where the provided vector is Vector3.Zero,
        /// returning the provided <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value, Vector3 fallback)
        {
            return value == Vector3.Zero ? fallback : Vector3.Normalize(value);
        }

        public static Vector2 ClampMagnitude(Vector2 value, float maxLength)
        {
            return ClampMagnitude(value, 0, maxLength);
        }

        public static Vector2 ClampMagnitude(Vector2 value, float minLength, float maxLength)
        {
            return value.AsNormalizedSafe() * Clamp(value.Length(), minLength, maxLength);
        }

        public static Vector3 ClampMagnitude(Vector3 value, float maxLength)
        {
            return ClampMagnitude(value, 0, maxLength);
        }

        public static Vector3 ClampMagnitude(Vector3 value, float minLength, float maxLength)
        {
            return value.AsNormalizedSafe() * Clamp(value.Length(), minLength, maxLength);
        }

        /// <summary>
        /// Swaps the component values of a <see cref="Vector3"/> from XYZ to
        /// the given format.
        /// </summary>
        public static Vector3 Swizzle(this Vector3 vector, Vector3Swizzle swizzle)
        {
            return swizzle switch
            {
                Vector3Swizzle.XYZ => vector,
                Vector3Swizzle.XZY => new Vector3(vector.X, vector.Z, vector.Y),
                Vector3Swizzle.YXZ => new Vector3(vector.Y, vector.X, vector.Z),
                Vector3Swizzle.YZX => new Vector3(vector.Y, vector.Z, vector.X),
                Vector3Swizzle.ZXY => new Vector3(vector.Z, vector.X, vector.Y),
                Vector3Swizzle.ZYX => new Vector3(vector.Z, vector.Y, vector.X),
                _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
            };
        }

        /// <summary>
        /// Opposite of Swizzle. Swaps the component values of a
        /// <see cref="Vector3"/> in the given format back to XYZ.
        /// </summary>
        public static Vector3 InverseSwizzle(this Vector3 vector, Vector3Swizzle swizzle)
        {
            return swizzle switch
            {
                Vector3Swizzle.XYZ => vector,
                Vector3Swizzle.XZY => new Vector3(vector.X, vector.Z, vector.Y),
                Vector3Swizzle.YXZ => new Vector3(vector.Y, vector.X, vector.Z),
                Vector3Swizzle.YZX => new Vector3(vector.Z, vector.X, vector.Y),
                Vector3Swizzle.ZXY => new Vector3(vector.Y, vector.Z, vector.X),
                Vector3Swizzle.ZYX => new Vector3(vector.Z, vector.Y, vector.X),
                _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
            };
        }

        /// <summary>
        /// Clamps the <see cref="Vector2"/> to the bounds given by
        /// <see cref="min"/> and <see cref="max"/>.
        /// </summary>
        public static void Clamp(ref this Vector2 vector, Vector2 min, Vector2 max)
        {
            vector.X = Clamp(vector.X, min.X, max.X);
            vector.Y = Clamp(vector.Y, min.Y, max.Y);
        }

        /// <summary>
        /// Clamps the <see cref="Vector3"/> to the bounds given by
        /// <see cref="min"/> and <see cref="max"/>.
        /// </summary>
        public static void Clamp(ref this Vector3 vector, Vector3 min, Vector3 max)
        {
            vector.X = Clamp(vector.X, min.X, max.X);
            vector.Y = Clamp(vector.Y, min.Y, max.Y);
            vector.Z = Clamp(vector.Z, min.Z, max.Z);
        }

        #endregion

        #region Vector Conversion

        // Vector component count conversion
        // The goal is to keep these conversions minimal
        // so only the most common cases are handled

        // Vector2 <- Vector3

        public static Vector2 Xy(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        // Vector2 -> Vector3

        public static Vector3 Xy0(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        public static Vector3 Xy1(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 1);
        }

        // Vector3 <- Vector4

        public static Vector3 Xyz(this Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        // Vector3 -> Vector4

        public static Vector4 Xyz0(this Vector3 value)
        {
            return new Vector4(value.X, value.Y, value.Z, 0);
        }

        public static Vector4 Xyz1(this Vector3 value)
        {
            return new Vector4(value.X, value.Y, value.Z, 1);
        }

        // Vector2Int <- Vector3Int

        public static Vector2Int Xy(this Vector3Int value)
        {
            return new Vector2Int(value.X, value.Y);
        }

        // Vector2Int -> Vector3Int

        public static Vector3Int Xy0(this Vector2Int value)
        {
            return new Vector3Int(value.X, value.Y, 0);
        }

        public static Vector3Int Xy1(this Vector2Int value)
        {
            return new Vector3Int(value.X, value.Y, 1);
        }

        #endregion

        #region Matrices

        /// <summary>
        /// Same as <see cref="Matrix4x4.CreateOrthographic"/>, but allows for reversed near and far planes.
        /// </summary>
        public static Matrix4x4 CreateOrthographic(float width, float height, float nearPlane, float farPlane)
        {
            var range = 1 / (nearPlane - farPlane);

            return new Matrix4x4(
                2 / width, 0, 0, 0,
                0, 2 / height, 0, 0,
                0, 0, range, 0,
                0, 0, range * nearPlane, 1
            );
        }

        /// <summary>
        /// Same as <see cref="Matrix4x4.CreatePerspectiveFieldOfView"/>, but allows for reversed near and far planes.
        /// </summary>
        public static Matrix4x4 CreatePerspective(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, float.Pi);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlane, 0);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlane, 0);

            var height = 1 / float.Tan(fieldOfView * 0.5f);
            var width = height / aspectRatio;
            var range = float.IsPositiveInfinity(farPlane) ? -1 : farPlane / (nearPlane - farPlane);

            return new Matrix4x4(
                width, 0, 0, 0,
                0, height, 0, 0,
                0, 0, range, -1,
                0, 0, range * nearPlane, 0
            );
        }

        #endregion

        #region Planes

        /// <summary>
        /// Creates a plane from a position and a normal.
        /// </summary>
        /// <remarks>
        /// .NET doesn't provide this method of construction for some reason.
        /// </remarks>
        public static Plane CreatePlane(Vector3 normal, Vector3 position)
        {
            normal = normal.AsNormalizedSafe();
            var distance = -Vector3.Dot(normal, position);

            return new Plane(normal, distance);
        }

        /// <summary>
        /// Casts a ray against the specified plane.
        /// <para/>
        /// The ray's length is ignored during this check.
        /// </summary>
        public static bool Raycast(this Plane plane, Ray ray, out float distance)
        {
            var vdot = Vector3.Dot(ray.Direction, plane.Normal);
            var ndot = -Vector3.Dot(ray.Origin, plane.Normal) - plane.D;

            if (IsApproximatelyEqual(vdot, 0f))
            {
                distance = 0f;

                return false;
            }

            distance = ndot / vdot;

            return distance > 0f;
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

        public static bool IsApproximatelyEqual(float a, float b)
        {
            var maxAb = Max(Abs(a), Abs(b));

            return Abs(a - b) < Max(0.00000_1f /* 6 digits */ * maxAb, float.Epsilon * 8);
        }

        public static bool IsApproximatelyEqual(double a, double b)
        {
            var maxAb = Max(Abs(a), Abs(b));

            return Abs(a - b) < Max(0.00000_00000_00000_1 /* 15 digits */ * maxAb, double.Epsilon * 8);
        }

        public static bool IsApproximatelyEqual(Vector2 a, Vector2 b)
        {
            return IsApproximatelyEqual(a.X, b.X) && IsApproximatelyEqual(a.Y, b.Y);
        }

        public static bool IsApproximatelyEqual(Vector3 a, Vector3 b)
        {
            return IsApproximatelyEqual(a.X, b.X) && IsApproximatelyEqual(a.Y, b.Y) && IsApproximatelyEqual(a.Z, b.Z);
        }

        #endregion
    }
}

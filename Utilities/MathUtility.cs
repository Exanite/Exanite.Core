using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        // Note: Order between different value type overloads should go by:
        // First by: float, double, int, long
        // Then by: degrees, radians

        #region Ranges

        /// <summary>
        /// Remaps a value from one range to another.
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromRange = fromMax - fromMin;
            var toRange = toMax - toMin;

            return fromRange == 0 ? toMin : toRange * ((value - fromMin) / fromRange) + toMin;
        }

        public static double Remap(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            var fromRange = fromMax - fromMin;
            var toRange = toMax - toMin;

            return fromRange == 0 ? toMin : toRange * ((value - fromMin) / fromRange) + toMin;
        }

        /// <summary>
        /// Wraps a value between min and max values.
        /// </summary>
        public static float Wrap(float value, float min, float max)
        {
            return Modulo(value - min, max - min) + min;
        }

        public static double Wrap(double value, double min, double max)
        {
            return Modulo(value - min, max - min) + min;
        }

        public static int Wrap(int value, int min, int max)
        {
            return Modulo(value - min, max - min) + min;
        }

        public static long Wrap(long value, long min, long max)
        {
            return Modulo(value - min, max - min) + min;
        }

        /// <summary>
        /// Returns the true modulo of a value when divided by a divisor.
        /// </summary>
        public static float Modulo(float value, float divisor)
        {
            return (value % divisor + divisor) % divisor;
        }

        public static double Modulo(double value, double divisor)
        {
            return (value % divisor + divisor) % divisor;
        }

        public static int Modulo(int value, int divisor)
        {
            return (value % divisor + divisor) % divisor;
        }

        public static long Modulo(long value, long divisor)
        {
            return (value % divisor + divisor) % divisor;
        }

        #endregion

        #region Floats

        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Math.Sign(target - current) * maxDelta;
        }

        public static float MoveTowardsAngleDegrees(float current, float target, float maxDelta)
        {
            var deltaAngle = DeltaAngleDegrees(current, target);
            if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
            {
                return target;
            }

            target = current + deltaAngle;
            return MoveTowards(current, target, maxDelta);
        }

        public static float MoveTowardsAngleRadians(float current, float target, float maxDelta)
        {
            var deltaAngle = DeltaAngleRadians(current, target);
            if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
            {
                return target;
            }

            target = current + deltaAngle;
            return MoveTowards(current, target, maxDelta);
        }

        public static float SmoothDamp(float current, float target, float smoothTime, float deltaTime, ref float currentVelocity, float maxSpeed = float.PositiveInfinity)
        {
            // From https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs#L309
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = MathF.Max(0.0001f, smoothTime);
            var omega = 2f / smoothTime;

            var x = omega * deltaTime;
            var exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            var change = current - target;
            var originalTo = target;

            // Clamp maximum speed
            var maxChange = maxSpeed * smoothTime;
            change = Math.Clamp(change, -maxChange, maxChange);
            target = current - change;

            var temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            var output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0f == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        #endregion

        #region Integers

        /// <summary>
        /// Gets the nearest multiple to a value.
        /// </summary>
        /// <example>
        /// GetNearestMultiple(45, 11) will return 44.
        /// </example>
        public static int GetNearestMultiple(int value, int multiple)
        {
            var remainder = value % multiple;
            var result = value - remainder;

            if (remainder > multiple / 2)
            {
                result += multiple;
            }

            return result;
        }

        /// <summary>
        /// Gets the next power of two.
        /// </summary>
        /// <example>
        /// GetNextPowerOfTwo(16) will return 16 GetNextPowerOfTwo(5) will
        /// return 8.
        /// </example>
        public static int GetNextPowerOfTwo(int value)
        {
            var result = 2;
            while (result < value)
            {
                result <<= 1;
            }

            return result;
        }

        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }

        public static bool IsOdd(this int num)
        {
            return num % 2 != 0;
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning Vector2.Zero rather
        /// than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value)
        {
            return value.AsNormalizedSafe(Vector2.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value, Vector2 fallback)
        {
            return value == Vector2.Zero ? fallback : Vector2.Normalize(value);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector3.Zero, returning Vector3.Zero rather
        /// than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value)
        {
            return value.AsNormalizedSafe(Vector3.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector3.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value, Vector3 fallback)
        {
            return value == Vector3.Zero ? fallback : Vector3.Normalize(value);
        }

        public static Vector2 Xy(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Vector3 Xy0(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        public static Vector3 Xy1(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 1);
        }

        public static Vector2 ClampMagnitude(Vector2 value, float maxLength)
        {
            return ClampMagnitude(value, 0, maxLength);
        }

        public static Vector2 ClampMagnitude(Vector2 value, float minLength, float maxLength)
        {
            return value.AsNormalizedSafe() * Math.Clamp(value.Length(), minLength, maxLength);
        }

        public static Vector3 ClampMagnitude(Vector3 value, float maxLength)
        {
            return ClampMagnitude(value, 0, maxLength);
        }

        public static Vector3 ClampMagnitude(Vector3 value, float minLength, float maxLength)
        {
            return value.AsNormalizedSafe() * Math.Clamp(value.Length(), minLength, maxLength);
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

        public static Vector4 SrgbToLinear(Vector4 srgb)
        {
            return new Vector4(MathUtility.SrgbToLinear(srgb.X), MathUtility.SrgbToLinear(srgb.Y), MathUtility.SrgbToLinear(srgb.Z), srgb.W);
        }

        public static Vector4 LinearToSrgb(Vector4 srgb)
        {
            return new Vector4(MathUtility.LinearToSrgb(srgb.X), MathUtility.LinearToSrgb(srgb.Y), MathUtility.LinearToSrgb(srgb.Z), srgb.W);
        }

        #endregion

        #region Trigonometry

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        public static float Rad2Deg(float radians)
        {
            return radians * (180f / MathF.PI);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static float Deg2Rad(float degrees)
        {
            return degrees * (MathF.PI / 180f);
        }

        public static float DeltaAngleRadians(float current, float target)
        {
            var delta = Wrap(target - current, 0, 2 * MathF.PI);
            if (delta > MathF.PI)
            {
                delta -= 2 * MathF.PI;
            }

            return delta;
        }

        public static float DeltaAngleDegrees(float current, float target)
        {
            var delta = Wrap(target - current, 0, 360);
            if (delta > 180)
            {
                delta -= 360;
            }

            return delta;
        }

        #endregion

        #region IsApproximatelyEqual

        public static bool IsApproximatelyEqual(float a, float b)
        {
            var maxAb = MathF.Max(MathF.Abs(a), MathF.Abs(b));

            return MathF.Abs(a - b) < MathF.Max(0.00000_1f /* 6 digits */ * maxAb, float.Epsilon * 8);
        }

        public static bool IsApproximatelyEqual(double a, double b)
        {
            var maxAb = Math.Max(Math.Abs(a), Math.Abs(b));

            return Math.Abs(a - b) < Math.Max(0.00000_00000_00000_1 /* 15 digits */ * maxAb, double.Epsilon * 8);
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

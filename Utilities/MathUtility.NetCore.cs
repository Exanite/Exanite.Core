#if NETCOREAPP
using System;
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
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

        public static Vector4 SrgbToLinear(string htmlColor)
        {
            var maxValue = (float)byte.MaxValue;
            var color = ColorTranslator.FromHtml(htmlColor);

            return SrgbToLinear(new Vector4(color.R / maxValue, color.G / maxValue, color.B / maxValue, color.A / maxValue));
        }

        public static Vector4 SrgbToLinear(Vector4 srgb)
        {
            return new Vector4(MathUtility.SrgbToLinear(srgb.X), MathUtility.SrgbToLinear(srgb.Y), MathUtility.SrgbToLinear(srgb.Z), srgb.W);
        }

        public static Vector4 LinearToSrgb(Vector4 srgb)
        {
            return new Vector4(MathUtility.LinearToSrgb(srgb.X), MathUtility.LinearToSrgb(srgb.Y), MathUtility.LinearToSrgb(srgb.Z), srgb.W);
        }
    }
}
#endif

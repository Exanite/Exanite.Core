using System;

namespace Exanite.Core.Utilities
{
    public static class MathUtility
    {
        // Note: Order between different value type overloads should go by float, double, int, long

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

#if !UNITY_5_3_OR_NEWER
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        public static float Rad2Deg(float radians)
        {
            return radians * (180f / float.Pi);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static float Deg2Rad(float degrees)
        {
            return degrees * (float.Pi / 180f);
        }
#endif

        // sRGB-Linear conversion formulas are from: https://entropymine.com/imageworsener/srgbformula/

        /// <summary>
        /// Converts a sRGB [0, 1] color value to linear [0, 1].
        /// </summary>
        public static float SrgbToLinear(float value)
        {
            if (value <= 0.04045f) {
                return value / 12.92f;
            } else {
                return MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
            }
        }

        /// <summary>
        /// Converts a linear [0, 1] color value to sRGB [0, 1].
        /// </summary>
        public static float LinearToSrgb(float value)
        {
            if (value <= 0.0031308f) {
                return value * 12.92f;
            } else {
                return MathF.Pow(value, 1 / 2.4f) * 1.055f - 0.055f;
            }
        }
    }
}

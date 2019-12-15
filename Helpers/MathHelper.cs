namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Helper class for math
    /// </summary>
    public static class MathHelper
    {
        // note: order between different value type overloads should go by float, double, int, long

        /// <summary>
        /// Remaps a value from one range to another
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float fromRange = fromMax - fromMin;
            float toRange = toMax - toMin;
            return fromRange == 0 ? toMin : (toRange * ((value - fromMin) / fromRange)) + toMin;
        }

        /// <summary>
        /// Remaps a value from one range to another
        /// </summary>
        public static double Remap(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            double fromRange = fromMax - fromMin;
            double toRange = toMax - toMin;
            return fromRange == 0 ? toMin : (toRange * ((value - fromMin) / fromRange)) + toMin;
        }

        /// <summary>
        /// Wraps a value between min and max values
        /// </summary>
        public static float Wrap(float value, float min, float max)
        {
            return Modulo(value - min, max - min) + min;
        }

        /// <summary>
        /// Wraps a value between min and max values
        /// </summary>
        public static double Wrap(double value, double min, double max)
        {
            return Modulo(value - min, max - min) + min;
        }

        /// <summary>
        /// Wraps a value between min and max values
        /// </summary>
        public static int Wrap(int value, int min, int max)
        {
            return Modulo(value - min, max - min) + min;
        }

        /// <summary>
        /// Wraps a value between min and max values
        /// </summary>
        public static long Wrap(long value, long min, long max)
        {
            return Modulo(value - min, max - min) + min;
        }

        /// <summary>
        /// Returns the true modulo of a value when divided by a divisor
        /// </summary>
        public static float Modulo(float value, float divisor)
        {
            return ((value % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Returns the true modulo of a value when divided by a divisor
        /// </summary>
        public static double Modulo(double value, double divisor)
        {
            return ((value % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Returns the true modulo of a value when divided by a divisor
        /// </summary>
        public static int Modulo(int value, int divisor)
        {
            return ((value % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Returns the true modulo of a value when divided by a divisor
        /// </summary>
        public static long Modulo(long value, long divisor)
        {
            return ((value % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Clamps a value between min and max values
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Clamps a value between min and max values
        /// </summary>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Clamps a value between min and max values
        /// </summary>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Clamps a value between min and max values
        /// </summary>
        public static long Clamp(long value, long min, long max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Gets the nearest multiple to a value <para/> 
        /// Example: GetNearestMultiple(45, 11) would return 44;
        /// </summary>
        public static int GetNearestMultiple(int value, int multiple)
        {
            int remainder = value % multiple;
            int result = value - remainder;

            if (remainder > (multiple / 2))
            {
                result += multiple;
            }

            return result;
        }

        /// <summary>
        /// Returns if the number is even
        /// </summary>
        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }

        /// <summary>
        /// Returns if the number is odd
        /// </summary>
        public static bool IsOdd(this int num)
        {
            return num % 2 != 0;
        }
    }
}
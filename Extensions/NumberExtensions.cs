namespace Exanite.Core.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Clamps a value between min and max values
        /// </summary>
        public static float Clamp(this float value, float min, float max)
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
        public static int Clamp(this int value, int min, int max)
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
        public static double Clamp(this double value, double min, double max)
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
        public static long Clamp(this long value, long min, long max)
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
        public static int GetNearestMultiple(this int value, int multiple)
        {
            int remainder = value % multiple;
            int result = value - remainder;

            if (remainder > (multiple / 2))
            {
                result += multiple;
            }

            return result;
        }

        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }

        public static bool IsOdd(this int num)
        {
            return !num.IsEven();
        }
    }
}
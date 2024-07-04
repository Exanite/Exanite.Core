#if NETCOREAPP
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        public static Vector4 SrgbToLinear(string htmlColor)
        {
            var maxValue = (float)byte.MaxValue;
            var color = ColorTranslator.FromHtml(htmlColor);

            return SrgbToLinear(new Vector4(color.R / maxValue, color.G / maxValue, color.B / maxValue, color.A / maxValue));
        }

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
    }
}
#endif

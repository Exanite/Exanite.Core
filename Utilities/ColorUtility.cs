#if NETCOREAPP
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities
{
    public static class ColorUtility
    {
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

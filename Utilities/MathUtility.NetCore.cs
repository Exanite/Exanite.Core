#if NETCOREAPP
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        #region Colors

        public static Vector4 SrgbToLinear(string htmlColor)
        {
            var maxValue = (float)byte.MaxValue;
            var color = ColorTranslator.FromHtml(htmlColor);

            return SrgbToLinear(new Vector4(color.R / maxValue, color.G / maxValue, color.B / maxValue, color.A / maxValue));
        }

        #endregion
    }
}
#endif

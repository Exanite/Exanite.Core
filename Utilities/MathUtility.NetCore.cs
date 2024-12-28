#if NETCOREAPP
using System;
using System.Drawing;
using System.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        #region Matrices

        /// <summary>
        /// Same as <see cref="Matrix4x4.CreateOrthographic"/>, but allows for reversed near and far planes.
        /// </summary>
        public static Matrix4x4 CreateOrthographic(float width, float height, float nearPlane, float farPlane)
        {
            var range = 1.0f / (nearPlane - farPlane);

            return new Matrix4x4(
                2.0f / width, 0, 0, 0,
                0, 2.0f / height, 0, 0,
                0, 0, range, 0,
                0, 0, range * nearPlane, 1
            );
        }

        /// <summary>
        /// Same as <see cref="Matrix4x4.CreatePerspectiveFieldOfView"/>, but allows for reversed near and far planes.
        /// </summary>
        public static Matrix4x4 CreatePerspective(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0.0f);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, float.Pi);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlane, 0.0f);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlane, 0.0f);

            var height = 1.0f / float.Tan(fieldOfView * 0.5f);
            var width = height / aspectRatio;
            var range = float.IsPositiveInfinity(farPlane) ? -1.0f : farPlane / (nearPlane - farPlane);

            return new Matrix4x4(
                width, 0, 0, 0,
                0, height, 0, 0,
                0, 0, range, -1.0f,
                0, 0, range * nearPlane, 0
            );
        }

        #endregion

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

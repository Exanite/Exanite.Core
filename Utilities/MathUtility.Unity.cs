#if UNITY_2021_3_OR_NEWER
using Exanite.Core.Numerics;
using UnityEngine;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        #region IsApproximatelyEqual

        public static bool IsApproximatelyEqual(Vector2 a, Vector2 b)
        {
            return IsApproximatelyEqual(a.x, b.x) && IsApproximatelyEqual(a.y, b.y);
        }

        public static bool IsApproximatelyEqual(Vector3 a, Vector3 b)
        {
            return IsApproximatelyEqual(a.x, b.x) && IsApproximatelyEqual(a.y, b.y) && IsApproximatelyEqual(a.z, b.z);
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Swaps the component values of a <see cref="Vector3"/> from XYZ to
        /// the given format.
        /// </summary>
        public static Vector3 Swizzle(this Vector3 vector3, Vector3Swizzle swizzle)
        {
            return swizzle switch
            {
                Vector3Swizzle.XYZ => vector3,
                Vector3Swizzle.XZY => new Vector3(vector3.x, vector3.z, vector3.y),
                Vector3Swizzle.YXZ => new Vector3(vector3.y, vector3.x, vector3.z),
                Vector3Swizzle.YZX => new Vector3(vector3.y, vector3.z, vector3.x),
                Vector3Swizzle.ZXY => new Vector3(vector3.z, vector3.x, vector3.y),
                Vector3Swizzle.ZYX => new Vector3(vector3.z, vector3.y, vector3.x),
                _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
            };
        }

        /// <summary>
        /// Opposite of Swizzle. Swaps the component values of a
        /// <see cref="Vector3"/> in the given format back to XYZ.
        /// </summary>
        public static Vector3 InverseSwizzle(this Vector3 vector3, Vector3Swizzle swizzle)
        {
            return swizzle switch
            {
                Vector3Swizzle.XYZ => vector3,
                Vector3Swizzle.XZY => new Vector3(vector3.x, vector3.z, vector3.y),
                Vector3Swizzle.YXZ => new Vector3(vector3.y, vector3.x, vector3.z),
                Vector3Swizzle.YZX => new Vector3(vector3.z, vector3.x, vector3.y),
                Vector3Swizzle.ZXY => new Vector3(vector3.y, vector3.z, vector3.x),
                Vector3Swizzle.ZYX => new Vector3(vector3.z, vector3.y, vector3.x),
                _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
            };
        }

        /// <summary>
        /// Returns the same <see cref="Vector3"/>, but with the X value set
        /// to the provided <paramref name="value"/>.
        /// </summary>
        public static Vector3 WithXAs(this Vector3 vector3, float value)
        {
            vector3.x = value;

            return vector3;
        }

        /// <summary>
        /// Returns the same <see cref="Vector3"/>, but with the Y value set
        /// to the provided <paramref name="value"/>.
        /// </summary>
        public static Vector3 WithYAs(this Vector3 vector3, float value)
        {
            vector3.y = value;

            return vector3;
        }

        /// <summary>
        /// Returns the same <see cref="Vector3"/>, but with the Z value set
        /// to the provided <paramref name="value"/>.
        /// </summary>
        public static Vector3 WithZAs(this Vector3 vector3, float value)
        {
            vector3.z = value;

            return vector3;
        }

        /// <summary>
        /// Clamps the <see cref="Vector3"/> to the bounds given by
        /// <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static void Clamp(ref this Vector3 vector3, Vector3 min, Vector3 max)
        {
            vector3.x = Mathf.Clamp(vector3.x, min.x, max.x);
            vector3.y = Mathf.Clamp(vector3.y, min.y, max.y);
            vector3.z = Mathf.Clamp(vector3.z, min.z, max.z);
        }

        /// <summary>
        /// Clamps the <see cref="Vector2"/> to the bounds given by
        /// <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static void Clamp(ref this Vector2 vector2, Vector2 min, Vector2 max)
        {
            vector2.x = Mathf.Clamp(vector2.x, min.x, max.x);
            vector2.y = Mathf.Clamp(vector2.y, min.y, max.y);
        }

        #endregion

        #region Rects

        public static Rect WithInset(this Rect rect, float inset)
        {
            rect.xMin += inset;
            rect.yMin += inset;

            rect.xMax -= inset;
            rect.yMax -= inset;

            return rect;
        }

        #endregion
    }
}
#endif

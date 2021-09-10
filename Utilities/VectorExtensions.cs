using System;
using Exanite.Core.Numbers;
using UnityEngine;

namespace Exanite.Core.Utilities
{
    /// <summary>
    ///     Extension methods for Unity's Vectors
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///     Swaps the component values of a <see cref="Vector3" /> from XYZ
        ///     to the given format
        /// </summary>
        public static Vector3 Swizzle(this Vector3 vector3, Vector3Swizzle swizzle)
        {
            switch (swizzle)
            {
                case Vector3Swizzle.XYZ:
                    return vector3;
                case Vector3Swizzle.XZY:
                    return new Vector3(vector3.x, vector3.z, vector3.y);
                case Vector3Swizzle.YXZ:
                    return new Vector3(vector3.y, vector3.x, vector3.z);
                case Vector3Swizzle.YZX:
                    return new Vector3(vector3.y, vector3.z, vector3.x);
                case Vector3Swizzle.ZXY:
                    return new Vector3(vector3.z, vector3.x, vector3.y);
                case Vector3Swizzle.ZYX:
                    return new Vector3(vector3.z, vector3.y, vector3.x);
            }

            throw new ArgumentException($"'{swizzle}' is not a valid swizzle", nameof(swizzle));
        }

        /// <summary>
        ///     Opposite of Swizzle. Swaps the component values of a
        ///     <see cref="Vector3" /> in the given format back to XYZ
        /// </summary>
        public static Vector3 InverseSwizzle(this Vector3 vector3, Vector3Swizzle swizzle)
        {
            switch (swizzle)
            {
                case Vector3Swizzle.XYZ:
                    return vector3;
                case Vector3Swizzle.XZY:
                    return new Vector3(vector3.x, vector3.z, vector3.y);
                case Vector3Swizzle.YXZ:
                    return new Vector3(vector3.y, vector3.x, vector3.z);
                case Vector3Swizzle.YZX:
                    return new Vector3(vector3.z, vector3.x, vector3.y);
                case Vector3Swizzle.ZXY:
                    return new Vector3(vector3.y, vector3.z, vector3.x);
                case Vector3Swizzle.ZYX:
                    return new Vector3(vector3.z, vector3.y, vector3.x);
            }

            throw new ArgumentException($"'{swizzle}' is not a valid swizzle", nameof(swizzle));
        }

        /// <summary>
        ///     Returns the same <see cref="Vector3" />, but with the X value set
        ///     to the provided <paramref name="value" />
        /// </summary>
        public static Vector3 WithXAs(this Vector3 vector3, float value)
        {
            vector3.x = value;

            return vector3;
        }

        /// <summary>
        ///     Returns the same <see cref="Vector3" />, but with the Y value set
        ///     to the provided <paramref name="value" />
        /// </summary>
        public static Vector3 WithYAs(this Vector3 vector3, float value)
        {
            vector3.y = value;

            return vector3;
        }

        /// <summary>
        ///     Returns the same <see cref="Vector3" />, but with the Z value set
        ///     to the provided <paramref name="value" />
        /// </summary>
        public static Vector3 WithZAs(this Vector3 vector3, float value)
        {
            vector3.z = value;

            return vector3;
        }

        /// <summary>
        ///     Clamps the <see cref="Vector3" /> to the bounds given by
        ///     <paramref name="min" /> and <paramref name="max" />
        /// </summary>
        public static void Clamp(ref this Vector3 vector3, Vector3 min, Vector3 max)
        {
            vector3.x = Mathf.Clamp(vector3.x, min.x, max.x);
            vector3.y = Mathf.Clamp(vector3.y, min.y, max.y);
            vector3.z = Mathf.Clamp(vector3.z, min.z, max.z);
        }

        /// <summary>
        ///     Clamps the <see cref="Vector2" /> to the bounds given by
        ///     <paramref name="min" /> and <paramref name="max" />
        /// </summary>
        public static void Clamp(ref this Vector2 vector2, Vector2 min, Vector2 max)
        {
            vector2.x = Mathf.Clamp(vector2.x, min.x, max.x);
            vector2.y = Mathf.Clamp(vector2.y, min.y, max.y);
        }
    }
}
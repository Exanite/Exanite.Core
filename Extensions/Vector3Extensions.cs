using System;
using Exanite.Core.Numbers;
using UnityEngine;

namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Vector3"/>s
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Swaps the component values of a <see cref="Vector3"/> from XYZ to the given format
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
        /// Opposite of Swizzle. Swaps the component values of a <see cref="Vector3"/> in the given format back to XYZ
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
    }
}

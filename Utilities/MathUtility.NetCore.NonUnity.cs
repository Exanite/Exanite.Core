#if NETCOREAPP && !UNITY_2021_3_OR_NEWER
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities
{
    public static partial class MathUtility
    {
        /// <summary>
        /// Creates a plane from a position and a normal.
        /// </summary>
        /// <remarks>
        /// .NET doesn't provide this method of construction for some reason.
        /// </remarks>
        public static Plane CreatePlane(Vector3 position, Vector3 normal) // Todo Verify that this is correct
        {
            normal = normal.AsNormalizedSafe();
            var distance = -Vector3.Dot(normal, position);

            return new Plane(normal, distance);
        }

        public static bool Raycast(this Plane plane, Ray ray, out float distance) // Todo Verify that this is correct
        {
            var vdot = Vector3.Dot(ray.DirectionMagnitude.AsNormalizedSafe(), plane.Normal);
            var ndot = -Vector3.Dot(ray.Origin, plane.Normal) - plane.D;

            if (IsApproximatelyEqual(vdot, 0f))
            {
                distance = 0.0F;

                return false;
            }

            distance = ndot / vdot;

            return distance > 0.0F;
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

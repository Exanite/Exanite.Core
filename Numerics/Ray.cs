using System;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics
{
    /// <remarks>
    /// This was added to support Jolt Physics, but can be used as a general use Ray struct.
    /// </remarks>
    public struct Ray
    {
        /// <remarks>
        /// Jolt uses a direction-magnitude vector for specifying rays.
        /// Using <see cref="float.MaxValue"/> will cause precision issues,
        /// so we limit the max length when using the distance-based constructor.
        /// </remarks>
        public const float MaxLength = 1_00000;

        public Vector3 Origin;
        public Vector3 DirectionMagnitude;

        public Ray(Vector3 origin, Vector3 directionMagnitude)
        {
            Origin = origin;
            DirectionMagnitude = directionMagnitude;
        }

        public Ray(Vector3 origin, Vector3 direction, float length) : this(origin, direction * MathF.Min(MaxLength, length)) {}

        public Vector3 GetPoint(float distance)
        {
            return Origin + distance * DirectionMagnitude.AsNormalizedSafe();
        }
    }
}

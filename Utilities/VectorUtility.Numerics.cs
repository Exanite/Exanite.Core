#if NETCOREAPP
using System.Numerics;

namespace Exanite.Core.Utilities
{
    // ReSharper disable once PartialTypeWithSinglePart

    /// <summary>
    /// Utility and extension methods for System.Numerics Vectors.
    /// </summary>
    public static partial class VectorUtility
    {
        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning Vector2.Zero rather
        /// than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value)
        {
            return value.AsNormalizedSafe(Vector2.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector2 AsNormalizedSafe(this Vector2 value, Vector2 fallback)
        {
            return value == Vector2.Zero ? fallback : Vector2.Normalize(value);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector3.Zero, returning Vector3.Zero rather
        /// than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value)
        {
            return value.AsNormalizedSafe(Vector3.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector3.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static Vector3 AsNormalizedSafe(this Vector3 value, Vector3 fallback)
        {
            return value == Vector3.Zero ? fallback : Vector3.Normalize(value);
        }

        public static Vector2 Xy(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Vector3 Xy0(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        public static Vector3 Xy1(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 1);
        }
    }
}
#endif

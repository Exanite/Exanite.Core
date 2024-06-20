#if NETCOREAPP
using System.Numerics;

namespace Exanite.Core.Utilities
{
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

        public static Vector2 Xy(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }
    }
}
#endif

using NumericsVector2 = System.Numerics.Vector2;

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
        public static NumericsVector2 AsNormalizedSafe(this NumericsVector2 value)
        {
            return value.AsNormalizedSafe(NumericsVector2.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static NumericsVector2 AsNormalizedSafe(this NumericsVector2 value, NumericsVector2 fallback)
        {
            return value == NumericsVector2.Zero ? fallback : NumericsVector2.Normalize(value);
        }
    }
}

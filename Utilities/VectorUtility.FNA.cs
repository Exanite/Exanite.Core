#if FNA
using FnaVector2 = Microsoft.Xna.Framework.Vector2;
#endif

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Utility and extension methods for FNA Vectors.
    /// </summary>
    public static partial class VectorUtility
    {
#if FNA
        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning Vector2.Zero rather
        /// than NaN.
        /// </summary>
        public static FnaVector2 AsNormalizedSafe(this FnaVector2 value)
        {
            return value.AsNormalizedSafe(FnaVector2.Zero);
        }

        /// <summary>
        /// Normalizes the vector. <br/> This handles the case where the
        /// provided vector is Vector2.Zero, returning the provided
        /// <see cref="fallback"/> rather than NaN.
        /// </summary>
        public static FnaVector2 AsNormalizedSafe(this FnaVector2 value, FnaVector2 fallback)
        {
            return value == FnaVector2.Zero ? fallback : FnaVector2.Normalize(value);
        }
#endif
    }
}

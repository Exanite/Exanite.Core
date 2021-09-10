using System.Collections.Generic;

namespace Exanite.Core.Utilities
{
    /// <summary>
    ///     Extension methods for <see cref="KeyValuePair{TKey,TValue}" />s
    /// </summary>
    public static class KeyValuePairExtensions
    {
        /// <summary>
        ///     Returns the reverse of <paramref name="pair" />
        /// </summary>
        public static KeyValuePair<TValue, TKey> AsReverse<TKey, TValue>(this KeyValuePair<TKey, TValue> pair)
        {
            return new KeyValuePair<TValue, TKey>(pair.Value, pair.Key);
        }
    }
}
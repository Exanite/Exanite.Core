using System.Diagnostics;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Asserts are disabled in Release mode. See <see cref="GuardUtility"/>.
    /// </remarks>
    public static class AssertUtility
    {
        public static T NotNull<T>(T? value) where T : notnull
        {
            Debug.Assert(value != null, "value != null");

            return value;
        }
    }
}

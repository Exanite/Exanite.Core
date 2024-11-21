using System.Diagnostics.CodeAnalysis;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Asserts are disabled in Release mode. See <see cref="GuardUtility"/>.
    /// </remarks>
    public static class AssertUtility
    {
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string? message = null)
        {
            GuardUtility.IsTrue(condition, message);
        }

        public static T NotNull<T>(T? value, string? message = null) where T : notnull
        {
#if DEBUG
            GuardUtility.NotNull(value, message);
#endif

            return value!;
        }
    }
}

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Asserts are disabled in Release mode. See <see cref="GuardUtility"/>.
    /// </remarks>
    public static class AssertUtility
    {
        [Conditional("DEBUG")]
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string message)
        {
            GuardUtility.IsTrue(condition, message);
        }

        [Conditional("DEBUG")]
        public static void IsFalse([DoesNotReturnIf(true)] bool condition, string message)
        {
            GuardUtility.IsFalse(condition, message);
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

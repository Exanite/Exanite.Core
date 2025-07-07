using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Asserts are disabled in Release mode. Also see <see cref="GuardUtility"/>.
    /// </remarks>
    public static class AssertUtility
    {
        [Conditional("DEBUG")]
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string errorMessage)
        {
            GuardUtility.IsTrue(condition, errorMessage);
        }

        [Conditional("DEBUG")]
        public static void IsFalse([DoesNotReturnIf(true)] bool condition, string errorMessage)
        {
            GuardUtility.IsFalse(condition, errorMessage);
        }

        public static T NotNull<T>(T? value, string? errorMessage = null) where T : notnull
        {
#if DEBUG
            GuardUtility.NotNull(value, errorMessage);
#endif

            return value!;
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Guards are always enabled. See <see cref="AssertUtility"/>.
    /// </remarks>
    public static class GuardUtility
    {
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string? message)
        {
            if (!condition)
            {
                throw new GuardException(message ?? "Condition is false");
            }
        }

        public static T NotNull<T>(T? value, string? message) where T : notnull
        {
            IsTrue(value != null, "Value is null");

            return value;
        }
    }
}

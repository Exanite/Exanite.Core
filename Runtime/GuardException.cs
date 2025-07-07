using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Exception thrown by <see cref="GuardUtility"/>.
    /// </summary>
    public class GuardException : Exception
    {
        public GuardException(string message) : base(message) {}
    }
}

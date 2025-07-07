using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Exception thrown by <see cref="AssertUtility"/>.
    /// </summary>
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message) {}
    }
}

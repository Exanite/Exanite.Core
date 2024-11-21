using System;

namespace Exanite.Core.Runtime
{
    public class GuardException : Exception
    {
        public GuardException(string message) : base(message) {}
    }
}

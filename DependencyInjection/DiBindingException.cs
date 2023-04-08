#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;

namespace Exanite.Core.DependencyInjection
{
    public class DiBindingException : Exception
    {
        public DiBindingException(string message) : base(message) {}
    }
}
#endif

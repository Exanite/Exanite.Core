using System;

namespace Plugins.Exanite.Core.DependencyInjection
{
    public class DiBindingException : Exception
    {
        public DiBindingException(string message) : base(message) {}
    }
}

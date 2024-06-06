using System;

namespace Exanite.Core.Utilities
{
    public class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; } = new();

        public void Dispose() {}
    }
}

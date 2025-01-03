using System;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// <see cref="IDisposable"/> implementation that does nothing.
    /// </summary>
    public class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; } = new();

        public void Dispose() {}
    }
}

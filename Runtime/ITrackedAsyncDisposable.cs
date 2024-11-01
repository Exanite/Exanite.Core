using System;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// See <see cref="ITrackedDisposable"/>.
    /// </summary>
    public interface ITrackedAsyncDisposable : IAsyncDisposable
    {
        public bool IsDisposed { get; }
    }
}

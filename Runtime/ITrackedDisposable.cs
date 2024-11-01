using System;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Same as <see cref="IDisposable"/>, but with an extra <see cref="IsDisposed"/> property.
    /// </summary>
    /// <remarks>
    /// This is mainly used to ensure <see cref="IDisposable"/> APIs are implemented consistently.
    /// </remarks>
    public interface ITrackedDisposable : IDisposable
    {
        public bool IsDisposed { get; }
    }
}

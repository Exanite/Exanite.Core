using System;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Same as <see cref="IDisposable"/>, but with an extra <see cref="IsDisposed"/> property.
    /// </summary>
    /// <remarks>
    /// This is mainly used to ensure <see cref="IDisposable"/> APIs are implemented consistently.
    /// <para/>
    /// Only use for classes that can only be disposed once.
    /// </remarks>
    public interface ITrackedDisposable : IDisposable
    {
        public bool IsDisposed { get; }

#if NEVER_COMPILE
        // Reference ITrackedDisposable implementation

        public bool IsDisposed { get; private set; }

        private unsafe void ReleaseUnmanagedResources()
        {

        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ITrackedDisposable()
        {
            ReleaseUnmanagedResources();
        }
#endif
    }
}

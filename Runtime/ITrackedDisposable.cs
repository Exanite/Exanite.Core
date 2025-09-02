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

#if EXANITE_NEVER_COMPILE
        // Reference ITrackedDisposable implementation

        public bool IsDisposed { get; private set; }

        private unsafe void ReleaseResources()
        {

        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        ~ITrackedDisposable()
        {
            Dispose();
        }
#endif
    }
}

using System.Threading;
using Exanite.Core.Runtime;

namespace Exanite.Core.Threading;

// Based off of https://github.com/martindevans/Myriad.ECS/blob/4e7226596dcd6aee60cc96118f535807545a714c/Myriad.ECS/Locks/RWLock.cs
// Original source is available under the MIT license
public class RwLock<T>
{
    private T value;
    private readonly ReaderWriterLockSlim sync = new(LockRecursionPolicy.SupportsRecursion);

    public RwLock(T value)
    {
        this.value = value;
    }

    public ReadLockHandle EnterReadLock(out ReadOnlyVRef<T> value)
    {
        sync.EnterReadLock();

        value = new ReadOnlyVRef<T>(ref this.value);
        return new ReadLockHandle(sync);
    }

    public WriteLockHandle EnterWriteLock(out VRef<T> value)
    {
        sync.EnterWriteLock();

        value = new VRef<T>(ref this.value);
        return new WriteLockHandle(sync);
    }

    public readonly ref struct ReadLockHandle
    {
        private readonly ReaderWriterLockSlim sync;

        internal ReadLockHandle(ReaderWriterLockSlim sync)
        {
            this.sync = sync;
        }

        public void Dispose()
        {
            sync.ExitReadLock();
        }
    }

    public readonly ref struct WriteLockHandle
    {
        private readonly ReaderWriterLockSlim sync;

        internal WriteLockHandle(ReaderWriterLockSlim sync)
        {
            this.sync = sync;
        }

        public void Dispose()
        {
            sync.ExitWriteLock();
        }
    }
}

using System;
using System.Threading;
using Exanite.Core.Utilities;

namespace Exanite.Core.Threading;

/// <summary>
/// Lightweight read-write guard for validating that writes never occur while reads are occurring.
/// Throws when failing to enter the guarded region.
/// Read recursion is supported. Write recursion is not supported.
/// </summary>
public class ReadWriteGuard
{
    private const int FreeState = 0;
    private const int WriteState = -1;

    /// <summary>
    /// -1 means write is acquired. 0 means free. >0 means read is acquired.
    /// </summary>
    private int state;

    public ReadGuardHandle EnterReadGuard()
    {
        var spin = new SpinWait();
        while (true)
        {
            var localState = Volatile.Read(ref state);
            GuardUtility.IsTrue(localState != WriteState, "Failed to enter read guard. Write guard is currently acquired");

            if (Interlocked.CompareExchange(ref state, localState + 1, localState) == localState)
            {
                break;
            }

            spin.SpinOnce();
        }

        return new ReadGuardHandle(this);
    }

    public WriteGuardHandle EnterWriteGuard()
    {
        var originalState = Interlocked.CompareExchange(ref state, WriteState, FreeState);
        GuardUtility.IsTrue(originalState == FreeState, "Failed to enter write guard. Read guard is currently acquired");

        return new WriteGuardHandle(this);
    }

    public readonly ref struct ReadGuardHandle : IDisposable
    {
        private readonly ReadWriteGuard guard;

        internal ReadGuardHandle(ReadWriteGuard guard)
        {
            this.guard = guard;
        }

        public void Dispose()
        {
            var localState = Interlocked.Decrement(ref guard.state);
            GuardUtility.IsTrue(localState != WriteState, "Failed to exit read guard");
        }
    }

    public readonly ref struct WriteGuardHandle : IDisposable
    {
        private readonly ReadWriteGuard guard;

        internal WriteGuardHandle(ReadWriteGuard guard)
        {
            this.guard = guard;
        }

        public void Dispose()
        {
            var originalState = Interlocked.CompareExchange(ref guard.state, FreeState, WriteState);
            GuardUtility.IsTrue(originalState == WriteState, "Failed to exit write guard");
        }
    }
}

using System;
using System.Threading;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

public class RefCounter : IRefCounted, ITrackedDisposable
{
    private readonly Lock sync = new();

    private readonly Action? onSetup;
    private readonly Action? onTeardown;

    public bool IsAlive { get; private set; }
    public bool IsDisposed { get; private set; }

    public uint RefCount { get; private set; }
    public bool IsReusable { get; }

    public RefCounter(uint initialRefCount, bool isReusable, Action? onSetup, Action? onTeardown)
    {
        RefCount = initialRefCount;
        IsReusable = isReusable;

        this.onSetup = onSetup;
        this.onTeardown = onTeardown;

        if (initialRefCount > 0)
        {
            Setup();
        }
    }

    public void AddRef()
    {
        lock (sync)
        {
            GuardUtility.IsFalse(IsDisposed, "Already disposed");

            if (RefCount == 0)
            {
                Setup();
            }

            RefCount++;
        }
    }

    public void RemoveRef()
    {
        lock (sync)
        {
            GuardUtility.IsFalse(IsDisposed, "Already disposed");
            GuardUtility.IsTrue(RefCount != 0, "Ref count is already 0");

            RefCount--;

            if (RefCount == 0)
            {
                Teardown();
            }
        }
    }

    public void SetRefCount(uint value)
    {
        lock (sync)
        {
            RefCount = value;

            if (RefCount == 0)
            {
                Teardown();
            }
            else
            {
                Setup();
            }
        }
    }

    public void Reset()
    {
        lock (sync)
        {
            Teardown();
            IsDisposed = false;
        }
    }

    public void Dispose()
    {
        lock (sync)
        {
            if (IsDisposed)
            {
                return;
            }

            Teardown();
            GC.SuppressFinalize(this);
        }
    }

    // Idempotent
    private void Setup()
    {
        GuardUtility.IsFalse(IsDisposed, "Already disposed");

        if (IsAlive)
        {
            return;
        }

        IsAlive = true;
        onSetup?.Invoke();
    }

    // Idempotent
    private void Teardown()
    {
        GuardUtility.IsFalse(IsDisposed, "Already disposed");

        if (!IsAlive)
        {
            return;
        }

        onTeardown?.Invoke();
        IsAlive = false;
        RefCount = 0;

        if (!IsReusable)
        {
            IsDisposed = true;
        }
    }
}

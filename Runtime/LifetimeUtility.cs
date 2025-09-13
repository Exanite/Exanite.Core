using System;

namespace Exanite.Core.Runtime;

public static class LifetimeUtility
{
    /// <summary>
    /// Adds a disposable object for disposal when the lifetime is disposed.
    /// </summary>
    public static T DisposeWith<T>(this T disposable, Lifetime lifetime) where T : IDisposable
    {
        return lifetime.DisposeWith(disposable);
    }

    /// <summary>
    /// Adds a ref to a ref countable object and removes the ref when the lifetime is disposed.
    /// </summary>
    public static T RefWith<T>(this T refCountable, Lifetime lifetime) where T : IRefCountable
    {
        refCountable.AddRef();

        return lifetime.RemoveRefWith(refCountable);
    }

    /// <summary>
    /// Removes a ref when the lifetime is disposed.
    /// </summary>
    public static T RemoveRefWith<T>(this T refCountable, Lifetime lifetime) where T : IRefCountable
    {
        return lifetime.RemoveRefWith(refCountable);
    }

    /// <summary>
    /// Adds an action to be invoked when the lifetime is disposed.
    /// </summary>
    public static Action InvokeWith(this Action action, Lifetime lifetime)
    {
        return lifetime.InvokeWith(action);
    }
}

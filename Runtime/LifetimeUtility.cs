using System;

namespace Exanite.Core.Runtime;

public static class LifetimeUtility
{
    /// <summary>
    /// Adds a disposable object for disposal when the lifetime is disposed.
    /// </summary>
    public static T DisposeWith<T>(this T disposable, Lifetime lifetime) where T : IDisposable
    {
        return lifetime.Add(disposable);
    }

    /// <summary>
    /// Adds an action to be invoked when the lifetime is disposed.
    /// </summary>
    public static Action InvokeWith(this Action action, Lifetime lifetime)
    {
        return lifetime.Add(action);
    }
}

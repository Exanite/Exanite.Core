using System;
using System.Collections.Generic;

namespace Exanite.Core.Runtime;

/// <summary>
/// Represents the lifetime of a collection of objects.
/// When this lifetime is disposed, all objects managed by this lifetime will also be disposed.
/// Objects are disposed in last-in first-out order (stack order).
/// </summary>
public class Lifetime : IDisposable
{
    // Either IDisposable or Action
    private readonly Stack<object> stack = new();

    internal T Add<T>(T disposable) where T : IDisposable
    {
        stack.Push(disposable);

        return disposable;
    }

    internal Action Add(Action action)
    {
        stack.Push(action);

        return action;
    }

    public void Dispose()
    {
        while (stack.TryPop(out var value))
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (value is Action action)
            {
                action.Invoke();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// Represents the lifetime of a collection of objects.
/// When this lifetime is disposed, all objects managed by this lifetime will also be disposed.
/// Objects are disposed in last-in first-out order (stack order).
/// </summary>
public class Lifetime : IDisposable
{
    private enum RegistrationType : byte
    {
        Disposable,
        RefCountable,
        Action,
    }

    private readonly Stack<RegistrationType> registrations = new();

    private readonly Stack<IDisposable> disposables = new();
    private readonly Stack<IRefCountable> refCountables = new();
    private readonly Stack<Action> actions = new();

    internal T DisposeWith<T>(T disposable) where T : IDisposable
    {
        registrations.Push(RegistrationType.Disposable);
        disposables.Push(disposable);

        return disposable;
    }

    internal T RemoveRefWith<T>(T refCountable) where T : IRefCountable
    {
        registrations.Push(RegistrationType.RefCountable);
        refCountables.Push(refCountable);

        return refCountable;
    }

    internal Action InvokeWith(Action action)
    {
        registrations.Push(RegistrationType.Action);
        actions.Push(action);

        return action;
    }

    public void Dispose()
    {
        while (registrations.TryPop(out var registrationType))
        {
            switch (registrationType)
            {
                case RegistrationType.Disposable:
                {
                    disposables.Pop().Dispose();
                    break;
                }
                case RegistrationType.RefCountable:
                {
                    refCountables.Pop().RemoveRef();
                    break;
                }
                case RegistrationType.Action:
                {
                    actions.Pop().Invoke();
                    break;
                }
                default:
                {
                    throw ExceptionUtility.NotSupportedEnumValue(registrationType);
                }
            }
        }
    }
}

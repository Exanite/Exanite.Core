using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;

namespace Exanite.Core.Events;

/// <summary>
/// Simple event bus. Can route events to other event buses.
/// </summary>
/// <remarks>
/// A hierarchy of event buses can be made by calling <c>senderEventBus.RegisterAny(receiverEventBus)</c>.
/// The receiver event bus will then receive all events received by the sender.
/// <para/>
/// Structs can be used and will not be boxed.
/// </remarks>
public class EventBus : IAllEventHandler, IDisposable
{
    private readonly List<IAllEventHandler> allHandlers = new();
    private readonly List<object?> invokerByTypeIndex = new();

    /// <summary>
    /// Configures all events received by this event bus to be sent to the provided handler.
    /// </summary>
    public void RegisterForwardAllTo(IAllEventHandler handler)
    {
        allHandlers.Add(handler);
    }

    /// <summary>
    /// Removes a handler registered by <see cref="RegisterForwardAllTo"/>.
    /// </summary>
    public bool UnregisterForwardAllTo(IAllEventHandler handler)
    {
        return allHandlers.Remove(handler);
    }

    /// <summary>
    /// Configures events of the specified type to be sent to the provided handler.
    /// </summary>
    public void Register<T>(IEventHandler<T> handler) where T : allows ref struct
    {
        Register<T>(handler.OnEvent);
    }

    /// <summary>
    /// Configures events of the specified type to be sent to the provided handler.
    /// </summary>
    public void Register<T>(Action<T> handler) where T : allows ref struct
    {
        CollectionsMarshal.SetCount(invokerByTypeIndex, TypeIndex.Get<T>() + 1);
        ref var invoker = ref invokerByTypeIndex.AsSpan()[TypeIndex.Get<T>()];
        invoker = Delegate.Combine((Action<T>?)invoker, handler);
    }

    /// <summary>
    /// Removes a handler registered by <see cref="Register{T}(IEventHandler{T})"/>.
    /// </summary>
    /// <returns>True if the handler was successfully removed.</returns>
    public bool Unregister<T>(IEventHandler<T> handler) where T : allows ref struct
    {
        return Unregister<T>(handler.OnEvent);
    }

    /// <summary>
    /// Removes a handler registered by <see cref="Register{T}(Action{T})"/>.
    /// </summary>
    /// <returns>True if the handler was successfully removed.</returns>
    public bool Unregister<T>(Action<T> handler) where T : allows ref struct
    {
        if ((uint)TypeIndex.Get<T>() >= (uint)invokerByTypeIndex.Count)
        {
            return false;
        }

        ref var invoker = ref invokerByTypeIndex.AsSpan()[TypeIndex.Get<T>()];
        var originalInvoker = invoker;
        invoker = Delegate.Remove((Action<T>?)invoker, handler);

        return originalInvoker != invoker;
    }

    /// <summary>
    /// Returns whether any event handlers are registered for the specified type.
    /// </summary>
    public bool HasHandlers<T>() where T : allows ref struct
    {
        if (allHandlers.Count > 0)
        {
            return true;
        }

        if ((uint)TypeIndex.Get<T>() >= (uint)invokerByTypeIndex.Count)
        {
            return false;
        }

        return invokerByTypeIndex[TypeIndex.Get<T>()] != null;
    }

    /// <summary>
    /// Raises an event.
    /// </summary>
    public void Raise<T>(T e) where T : allows ref struct
    {
        foreach (var anyHandler in allHandlers)
        {
            anyHandler.OnEvent(e);
        }

        if ((uint)TypeIndex.Get<T>() >= (uint)invokerByTypeIndex.Count)
        {
            return;
        }

        var invoker = invokerByTypeIndex[TypeIndex.Get<T>()];
        if (invoker != null)
        {
            Unsafe.As<object, Action<T>>(ref invoker).Invoke(e);
        }
    }

    /// <summary>
    /// Clears all registered event handlers.
    /// </summary>
    public void Clear()
    {
        allHandlers.Clear();
        invokerByTypeIndex.Clear();
    }

    public void Dispose()
    {
        Clear();
    }

    void IAllEventHandler.OnEvent<T>(T e)
    {
        Raise(e);
    }
}

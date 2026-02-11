using System;
using System.Collections.Generic;
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
    private readonly List<List<object>?> handlersByTypeIndex = new();

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
        CollectionsMarshal.SetCount(handlersByTypeIndex, TypeIndex.Get<T>() + 1);
        ref var handlers = ref handlersByTypeIndex.AsSpan()[TypeIndex.Get<T>()];
        if (handlers == null)
        {
            handlers = [];
        }

        handlers.Add(handler);
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
        if (handlersByTypeIndex.Count <= TypeIndex.Get<T>())
        {
            return false;
        }

        ref var handlers = ref handlersByTypeIndex.AsSpan()[TypeIndex.Get<T>()];
        if (handlers == null)
        {
            handlers = [];
        }

        return handlers.Remove(handler);
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

        if (handlersByTypeIndex.Count <= TypeIndex.Get<T>())
        {
            return;
        }

        ref var handlers = ref handlersByTypeIndex.AsSpan()[TypeIndex.Get<T>()];
        if (handlers != null)
        {
            foreach (var handler in handlers)
            {
                ((Action<T>)handler).Invoke(e);
            }
        }
    }

    /// <summary>
    /// Clears all registered event handlers.
    /// </summary>
    public void Clear()
    {
        allHandlers.Clear();
        handlersByTypeIndex.Clear();
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

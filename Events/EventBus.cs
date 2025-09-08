using System;
using System.Collections.Generic;

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
    private readonly Dictionary<Type, List<IAllEventHandler>> specificHandlersByType = new();
    private readonly Dictionary<Type, List<object>> handlersByType = new();

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
    public void RegisterForwardSpecificTo<T>(IAllEventHandler handler)
    {
        var type = typeof(T);

        if (!specificHandlersByType.ContainsKey(typeof(T)))
        {
            specificHandlersByType.Add(type, new List<IAllEventHandler>());
        }

        specificHandlersByType[type].Add(handler);
    }

    /// <summary>
    /// Removes a handler registered by <see cref="RegisterForwardSpecificTo{T}(IAllEventHandler)"/>.
    /// </summary>
    /// <returns>True if the handler was successfully removed.</returns>
    public bool UnregisterForwardSpecificTo<T>(IAllEventHandler handler)
    {
        if (!specificHandlersByType.TryGetValue(typeof(T), out var handlerList))
        {
            return false;
        }

        return handlerList.Remove(handler);
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
        var type = typeof(T);

        if (!handlersByType.ContainsKey(typeof(T)))
        {
            handlersByType.Add(type, new List<object>());
        }

        handlersByType[type].Add(handler);
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
        if (!handlersByType.TryGetValue(typeof(T), out var handlerList))
        {
            return false;
        }

        return handlerList.Remove(handler);
    }

    /// <summary>
    /// Raises an event.
    /// </summary>
    public void Raise<T>(T e) where T : allows ref struct
    {
        var type = typeof(T);

        foreach (var anyHandler in allHandlers)
        {
            anyHandler.OnEvent(e);
        }

        if (specificHandlersByType.TryGetValue(type, out var specificHandlerList))
        {
            foreach (var handler in specificHandlerList)
            {
                handler.OnEvent(e);
            }
        }

        if (handlersByType.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
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
        specificHandlersByType.Clear();
        handlersByType.Clear();
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
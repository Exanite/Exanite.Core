using System;
using System.Collections.Generic;

namespace Exanite.Core.Events
{
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
        private readonly Dictionary<Type, List<object>> handlerLists = new();

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
        public void Register<T>(IEventHandler<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            Register<T>(handler.OnEvent);
        }

        /// <summary>
        /// Configures events of the specified type to be sent to the provided handler.
        /// </summary>
        public void Register<T>(Action<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            var type = typeof(T);

            if (!handlerLists.ContainsKey(typeof(T)))
            {
                handlerLists.Add(type, new List<object>());
            }

            handlerLists[type].Add(handler);
        }

        /// <summary>
        /// Removes a handler registered by <see cref="Register{T}(IEventHandler{T})"/>.
        /// </summary>
        public bool Unregister<T>(IEventHandler<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            return Unregister<T>(handler.OnEvent);
        }

        /// <summary>
        /// Removes a handler registered by <see cref="Register{T}(Action{T})"/>.
        /// </summary>
        public bool Unregister<T>(Action<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            if (!handlerLists.TryGetValue(typeof(T), out var handlerList))
            {
                return false;
            }

            return handlerList.Remove(handler);
        }

        /// <summary>
        /// Raises an event.
        /// </summary>
        public void Raise<T>(T e)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            var type = typeof(T);

            foreach (var anyHandler in allHandlers)
            {
                anyHandler.OnEvent(e);
            }

            if (handlerLists.TryGetValue(type, out var handlerList))
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
            handlerLists.Clear();
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
}

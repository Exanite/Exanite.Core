using System;
using System.Collections.Generic;

namespace Exanite.Core.Events
{
    /// <summary>
    /// Synchronous event bus.
    /// </summary>
    /// <remarks>
    /// Hierarchies of event buses can be made by calling <c>childEventBus.RegisterAny(parentEventBus)</c>.
    /// The parent event bus will then receive all events received by the child.
    /// <para/>
    /// Structs can be used and will not be boxed.
    /// </remarks>
    public class EventBus : IAnyEventHandler, IDisposable
    {
        private readonly List<IAnyEventHandler> anyHandlers = new();
        private readonly Dictionary<Type, List<object>> handlerLists = new();

        public void RegisterAny(IAnyEventHandler handler)
        {
            anyHandlers.Add(handler);
        }

        public bool UnregisterAny(IAnyEventHandler handler)
        {
            return anyHandlers.Remove(handler);
        }

        public void Register<T>(IEventHandler<T> handler)
        {
            Register<T>(handler.OnEvent);
        }

        public void Register<T>(Action<T> handler)
        {
            var type = typeof(T);

            if (!handlerLists.ContainsKey(typeof(T)))
            {
                handlerLists.Add(type, new List<object>());
            }

            handlerLists[type].Add(handler);
        }

        public bool Unregister<T>(IEventHandler<T> handler)
        {
            return Unregister<T>(handler.OnEvent);
        }

        public bool Unregister<T>(Action<T> handler)
        {
            if (!handlerLists.TryGetValue(typeof(T), out var handlerList))
            {
                return false;
            }

            return handlerList.Remove(handler);
        }

        public void Raise<T>(T e)
        {
            var type = typeof(T);

            foreach (var anyHandler in anyHandlers)
            {
                anyHandler.OnAnyEvent(e);
            }

            if (handlerLists.TryGetValue(type, out var handlerList))
            {
                foreach (var handler in handlerList)
                {
                    ((Action<T>)handler).Invoke(e);
                }
            }
        }

        public void Clear()
        {
            anyHandlers.Clear();
            handlerLists.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        void IAnyEventHandler.OnAnyEvent<T>(T e)
        {
            Raise(e);
        }
    }
}

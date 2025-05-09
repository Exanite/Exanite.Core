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

        public void RegisterSendAllTo(IAllEventHandler handler)
        {
            allHandlers.Add(handler);
        }

        public bool UnregisterSendAllTo(IAllEventHandler handler)
        {
            return allHandlers.Remove(handler);
        }

        public void Register<T>(IEventHandler<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            Register<T>(handler.OnEvent);
        }

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

        public bool Unregister<T>(IEventHandler<T> handler)
#if NETCOREAPP
            where T : allows ref struct
#endif
        {
            return Unregister<T>(handler.OnEvent);
        }

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

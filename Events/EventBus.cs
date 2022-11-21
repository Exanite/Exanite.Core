using System;
using System.Collections.Generic;

namespace Exanite.Core.Events
{
    public class EventBus : IAnyEventListener, IDisposable
    {
        private readonly List<IAnyEventListener> anyListeners = new();
        private readonly Dictionary<Type, List<object>> listenerLists = new();

        public void SubscribeAny(IAnyEventListener listener)
        {
            anyListeners.Add(listener);
        }

        public bool UnsubscribeAny(IAnyEventListener listener)
        {
            return anyListeners.Remove(listener);
        }

        public void Subscribe<T>(IEventListener<T> listener)
        {
            var type = typeof(T);

            if (!listenerLists.ContainsKey(typeof(T)))
            {
                listenerLists.Add(type, new List<object>());
            }

            listenerLists[type].Add(listener);
        }

        public bool Unsubscribe<T>(IEventListener<T> listener)
        {
            if (!listenerLists.TryGetValue(typeof(T), out var listenerList))
            {
                return false;
            }

            return listenerList.Remove(listener);
        }

        public void Publish<T>(T e)
        {
            var type = typeof(T);

            foreach (var anyListener in anyListeners)
            {
                anyListener.OnAnyEvent(e);
            }

            if (listenerLists.TryGetValue(type, out var listenerList))
            {
                foreach (var listener in listenerList)
                {
                    ((IEventListener<T>)listener).OnEvent(e);
                }
            }
        }

        public void Clear()
        {
            anyListeners.Clear();
            listenerLists.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
        }

        void IAnyEventListener.OnAnyEvent<T>(T e)
        {
            Publish(e);
        }
    }
}
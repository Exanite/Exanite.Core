using System;
using System.Collections.Generic;

namespace Exanite.Core.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<object>> listenerLists = new Dictionary<Type, List<object>>();

        public void Subscribe<T>(IEventListener<T> listener)
        {
            var type = typeof(T);

            if (!listenerLists.ContainsKey(typeof(T)))
            {
                listenerLists.Add(type, new List<object>());
            }

            listenerLists[type].Add(listener);
        }

        public void Unsubscribe<T>(IEventListener<T> listener)
        {
            if (!listenerLists.TryGetValue(typeof(T), out var listenerList))
            {
                return;
            }

            listenerList.Remove(listener);
        }

        public void Publish<T>(T e)
        {
            var type = typeof(T);

            if (!listenerLists.TryGetValue(type, out var listenerList))
            {
                return;
            }

            foreach (var listener in listenerList)
            {
                ((IEventListener<T>)listener).OnEvent(e);
            }
        }
    }
}
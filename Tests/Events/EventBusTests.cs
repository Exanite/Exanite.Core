using System;
using Exanite.Core.Events;
using NUnit.Framework;
#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

namespace Exanite.Core.Tests.Events
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public void Unregister_Works_WhenUsingInterface()
        {
            var eventBus = new EventBus();

            var didReceiveEvent = false;
            var eventHandler = new EventHandler<Event>(_ =>
            {
                didReceiveEvent = true;
            });

            // Should receive event when registered
            eventBus.Register(eventHandler);
            eventBus.Raise(new Event());

            Assert.IsTrue(didReceiveEvent);

            // Should not receive when unregistered
            didReceiveEvent = false;
            eventBus.Unregister(eventHandler);
            eventBus.Raise(new Event());

            Assert.IsFalse(didReceiveEvent);
        }

        public struct Event {}

        public class EventHandler<T> : IEventHandler<T>
        {
            private readonly Action<T> handleEvent;

            public EventHandler(Action<T> handleEvent)
            {
                this.handleEvent = handleEvent;
            }

            public void OnEvent(T e)
            {
                handleEvent(e);
            }
        }
    }
}

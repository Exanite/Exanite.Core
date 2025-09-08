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
            var eventHandler = new EventHandler<Event>();

            // Should receive event when registered
            eventBus.Register(eventHandler);
            eventBus.Raise(new Event());

            Assert.IsTrue(eventHandler.ReceiveCount == 1);

            // Should not receive when unregistered
            eventBus.Unregister(eventHandler);
            eventBus.Raise(new Event());

            Assert.IsTrue(eventHandler.ReceiveCount == 1);
        }

        private struct Event {}

        private class EventHandler<T> : IEventHandler<T>
        {
            public int ReceiveCount { get; private set; }

            public void OnEvent(T e)
            {
                ReceiveCount++;
            }
        }
    }
}

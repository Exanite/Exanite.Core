using Exanite.Core.Events;
using NUnit.Framework;

namespace Exanite.Core.Tests.Events;

[TestFixture]
public class EventBusTests
{
    [Test]
    public void RegisterForwardTo_OnlyForwardsSpecifiedType()
    {
        var eventBus = new EventBus();
        var eventHandler = new AllEventHandler();

        // Should receive event when registered
        eventBus.RegisterForwardTo<Event>(eventHandler);
        eventBus.Raise(new Event());

        Assert.That(eventHandler.ReceiveCount, Is.EqualTo(1));

        // Should not receive unregistered event
        eventBus.Raise(new EventB());

        Assert.That(eventHandler.ReceiveCount, Is.EqualTo(1));

        // Should not receive event after unregistering
        eventBus.UnregisterForwardTo<Event>(eventHandler);
        eventBus.Raise(new Event());

        Assert.That(eventHandler.ReceiveCount, Is.EqualTo(1));
    }

    [Test]
    public void Unregister_SuccessfullyUnregisters_WhenUsingInterface()
    {
        var eventBus = new EventBus();
        var eventHandler = new EventHandler<Event>();

        // Should receive event when registered
        eventBus.Register(eventHandler);
        eventBus.Raise(new Event());

        Assert.That(eventHandler.ReceiveCount, Is.EqualTo(1));

        // Should not receive when unregistered
        eventBus.Unregister(eventHandler);
        eventBus.Raise(new Event());

        Assert.That(eventHandler.ReceiveCount, Is.EqualTo(1));
    }

    private struct Event;
    private struct EventB;

    private class EventHandler<T> : IEventHandler<T>
    {
        public int ReceiveCount { get; private set; }

        public void OnEvent(T e)
        {
            ReceiveCount++;
        }
    }

    private class AllEventHandler : IAllEventHandler
    {
        public int ReceiveCount { get; private set; }

        public void OnEvent<T>(T e) where T : allows ref struct
        {
            ReceiveCount++;
        }
    }
}
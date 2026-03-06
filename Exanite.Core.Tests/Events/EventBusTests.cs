using Exanite.Core.Events;
using Xunit;

namespace Exanite.Core.Tests.Events;

public class EventBusTests
{
    [Fact]
    public void Unregister_SuccessfullyUnregisters_WhenUsingInterface()
    {
        using var eventBus = new EventBus();
        var eventHandler = new EventHandler<Event>();

        // Should receive event when registered
        eventBus.Register(eventHandler);
        eventBus.Raise(new Event());

        Assert.Equal(1, eventHandler.ReceiveCount);

        // Should not receive when unregistered
        eventBus.Unregister(eventHandler);
        eventBus.Raise(new Event());

        Assert.Equal(1, eventHandler.ReceiveCount);
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

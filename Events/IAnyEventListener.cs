namespace Exanite.Core.Events
{
    public interface IAnyEventListener
    {
        void OnAnyEvent(object e);
    }
}
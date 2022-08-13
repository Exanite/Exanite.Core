namespace Exanite.Core.Events
{
    public interface IAnyEventListener
    {
        void OnAnyEvent<T>(T e);
    }
}
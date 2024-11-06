namespace Exanite.Core.Events
{
    public interface IAnyEventHandler
    {
        void OnAnyEvent<T>(T e);
    }
}

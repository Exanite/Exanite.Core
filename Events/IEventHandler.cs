namespace Exanite.Core.Events
{
    public interface IEventHandler<in T> where T : allows ref struct
    {
        void OnEvent(T e);
    }
}

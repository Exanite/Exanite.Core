namespace Exanite.Core.Events
{
    public interface IEventListener<in T>
    {
        void OnEvent(T e);
    }
}
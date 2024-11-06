namespace Exanite.Core.Events
{
    public interface IEventHandler<in T>
    {
        void OnEvent(T e);
    }
}

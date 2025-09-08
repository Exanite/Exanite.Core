namespace Exanite.Core.Events
{
    public interface IEventHandler<in T>
#if NETCOREAPP
    where T : allows ref struct
#endif
    {
        void OnEvent(T e);
    }
}

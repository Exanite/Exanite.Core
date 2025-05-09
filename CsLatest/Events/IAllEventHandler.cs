namespace Exanite.Core.Events
{
    public interface IAllEventHandler
    {
        void OnEvent<T>(T e)
#if NETCOREAPP
            where T : allows ref struct
#endif
        ;
    }
}

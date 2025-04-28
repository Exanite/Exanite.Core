namespace Exanite.Core.Events
{
    public interface IAnyEventHandler
    {
        void OnAnyEvent<T>(T e)
#if NETCOREAPP
            where T : allows ref struct
#endif
        ;
    }
}

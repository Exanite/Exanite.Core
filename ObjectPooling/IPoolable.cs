namespace Exanite.Core.ObjectPooling
{
    public interface IPoolable
    {
        void OnGet();

        void OnReleased();
    }
}

namespace Exanite.Core.Runtime
{
    public interface IRefCounted
    {
        public void AddRef();
        public void RemoveRef();
    }
}

namespace Exanite.Core.Runtime;

public interface IRefCountable
{
    public void AddRef();
    public void RemoveRef();
}

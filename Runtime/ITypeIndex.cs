namespace Exanite.Core.Runtime;

public interface ITypeIndex
{
    public static abstract int Get<T>() where T : allows ref struct;
}

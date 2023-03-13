#if EXANITE_UNIDI && ODIN_INSPECTOR
namespace Plugins.Exanite.Core.DependencyInjection
{
    public enum BindType
    {
        Self,
        AllInterfaces,
        AllInterfacesAndSelf,
        Custom,
    }
}
#endif
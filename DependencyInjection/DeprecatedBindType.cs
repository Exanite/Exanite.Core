using System;

#if EXANITE_UNIDI && ODIN_INSPECTOR
namespace Exanite.Core.DependencyInjection
{
    [Obsolete]
    public enum DeprecatedBindType
    {
        Self,
        AllInterfaces,
        AllInterfacesAndSelf,
        Custom,
    }

    [Flags]
    public enum BindType
    {
        None = 0,
        Self = 1 << 0,
        AllInterfaces = 1 << 1,
        Custom = 1 << 2,
    }
}
#endif

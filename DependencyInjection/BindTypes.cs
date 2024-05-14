using System;

#if EXANITE_UNIDI && ODIN_INSPECTOR
namespace Exanite.Core.DependencyInjection
{
    [Flags]
    public enum BindTypes
    {
        None = 0,
        Self = 1 << 0,
        Interfaces = 1 << 1,
        Custom = 1 << 2,
    }
}
#endif

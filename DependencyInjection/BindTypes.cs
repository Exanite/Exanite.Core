#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;

namespace Exanite.Core.DependencyInjection
{
    [Flags]
    public enum BindTypes
    {
        None = 0,
        Smart = 1 << 1 | Self | Interfaces,
        Self = 1 << 2,
        Interfaces = 1 << 3,
        Custom = 1 << 30,
    }
}
#endif

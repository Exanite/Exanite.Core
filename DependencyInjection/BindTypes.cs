#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;

namespace Exanite.Core.DependencyInjection
{
    [Flags]
    public enum BindTypes
    {
        None = 0,

        /// <summary>
        /// Binds the object's type, its implemented interfaces, and its inheritance hierarchy
        /// and stops once one of the types defined in <see cref="ComponentBinding.IgnoredTypes"/> are reached.
        /// </summary>
        Smart = 1 << 1 | Self | Interfaces,

        /// <summary>
        /// Binds the object's type.
        /// </summary>
        Self = 1 << 2,

        /// <summary>
        /// Binds the object's implemented interfaces.
        /// </summary>
        Interfaces = 1 << 3,

        /// <summary>
        /// Binds a custom list of types.
        /// </summary>
        Custom = 1 << 30,
    }
}
#endif

using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public static class TypeExpanderUtility
{
    public static IEnumerable<Type> Expand(this Type type, ITypeExpander expander)
    {
        return expander.Expand(type);
    }
}

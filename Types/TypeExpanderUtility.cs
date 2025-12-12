using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public static class TypeExpanderUtility
{
    public static IEnumerable<Type> Expand(this Type? type, ITypeExpander expander)
    {
        if (type == null)
        {
            return [];
        }

        return expander.Expand(type);
    }
}

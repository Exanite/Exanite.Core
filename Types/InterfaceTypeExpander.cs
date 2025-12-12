using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public class InterfaceTypeExpander : ITypeExpander
{
    public IEnumerable<Type> Expand(Type type)
    {
        return type.GetInterfaces();
    }
}

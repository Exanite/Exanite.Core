using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

/// <summary>
/// Type expander that returns all interfaces implemented by the input type.
/// </summary>
public class InterfaceTypeExpander : ITypeExpander
{
    public IEnumerable<Type> Expand(Type type)
    {
        return type.GetInterfaces();
    }
}

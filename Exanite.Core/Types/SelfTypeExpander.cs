using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

/// <summary>
/// Type expander that returns the input type.
/// </summary>
public class SelfTypeExpander : ITypeExpander
{
    public IEnumerable<Type> Expand(Type type)
    {
        yield return type;
    }
}

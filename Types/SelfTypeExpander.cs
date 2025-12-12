using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public class SelfTypeExpander : ITypeExpander
{
    public IEnumerable<Type> Expand(Type type)
    {
        yield return type;
    }
}

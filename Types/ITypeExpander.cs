using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public interface ITypeExpander
{
    public IEnumerable<Type> Expand(Type type);
}

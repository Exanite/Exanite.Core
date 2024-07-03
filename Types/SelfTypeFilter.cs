using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class SelfTypeFilter : ITypeFilter
    {
        public IEnumerable<Type> Filter(Type type)
        {
            yield return type;
        }
    }
}

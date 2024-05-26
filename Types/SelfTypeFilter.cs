using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class SelfTypeFilter : IInstanceTypeFilter
    {
        public IEnumerable<Type> Filter(Type type)
        {
            yield return type;
        }
    }
}
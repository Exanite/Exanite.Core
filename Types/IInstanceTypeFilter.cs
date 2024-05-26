using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public interface IInstanceTypeFilter
    {
        public IEnumerable<Type> Filter(Type type);
    }
}
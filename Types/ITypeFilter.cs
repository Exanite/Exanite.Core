using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public interface ITypeFilter
    {
        public IEnumerable<Type> Filter(Type type);
    }
}

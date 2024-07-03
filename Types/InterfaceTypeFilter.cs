using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class InterfaceTypeFilter : ITypeFilter
    {
        public IEnumerable<Type> Filter(Type type)
        {
            return type.GetInterfaces();
        }
    }
}

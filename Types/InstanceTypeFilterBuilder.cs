using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class InstanceTypeFilterBuilder
    {
        private List<IInstanceTypeFilter> filters = new();

        public InstanceTypeFilterBuilder Self()
        {
            filters.Add(new SelfTypeFilter());

            return this;
        }

        public InstanceTypeFilterBuilder Interfaces()
        {
            filters.Add(new InterfaceTypeFilter());

            return this;
        }

        public InstanceTypeFilterBuilder InheritanceHierarchy(Type baseType, bool inclusive = false, bool mustExtendBaseType = false)
        {
            filters.Add(new InheritanceHierarchyTypeFilter(baseType, inclusive, mustExtendBaseType));

            return this;
        }
    }
}

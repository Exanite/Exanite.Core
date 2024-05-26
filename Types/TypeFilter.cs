using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class TypeFilter : IInstanceTypeFilter
    {
        private List<IInstanceTypeFilter> filters = new();

        public TypeFilter Self()
        {
            filters.Add(new SelfTypeFilter());

            return this;
        }

        public TypeFilter Interfaces()
        {
            filters.Add(new InterfaceTypeFilter());

            return this;
        }

        public TypeFilter InheritanceHierarchy<TBaseType>(bool inclusive = false, bool mustExtendBaseType = false)
        {
            filters.Add(new InheritanceHierarchyTypeFilter(typeof(TBaseType), inclusive, mustExtendBaseType));

            return this;
        }

        public TypeFilter InheritanceHierarchy(Type baseType, bool inclusive = false, bool mustExtendBaseType = false)
        {
            filters.Add(new InheritanceHierarchyTypeFilter(baseType, inclusive, mustExtendBaseType));

            return this;
        }

        public IEnumerable<Type> Filter(Type type)
        {
            var results = new HashSet<Type>();
            foreach (var filter in filters)
            {
                foreach (var filterType in filter.Filter(type))
                {
                    results.Add(filterType);
                }
            }

            return results;
        }
    }
}

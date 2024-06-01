using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class TypeFilter : ITypeFilter
    {
        public List<ITypeFilter> Filters { get; } = new();

        public TypeFilter Self()
        {
            Filters.Add(new SelfTypeFilter());

            return this;
        }

        public TypeFilter Interfaces()
        {
            Filters.Add(new InterfaceTypeFilter());

            return this;
        }

        public TypeFilter InheritanceHierarchy<TBaseType>(bool inclusive = false, bool mustExtendBaseType = false)
        {
            Filters.Add(new InheritanceHierarchyTypeFilter(typeof(TBaseType), inclusive, mustExtendBaseType));

            return this;
        }

        public TypeFilter InheritanceHierarchy(Type baseType, bool inclusive = false, bool mustExtendBaseType = false)
        {
            Filters.Add(new InheritanceHierarchyTypeFilter(baseType, inclusive, mustExtendBaseType));

            return this;
        }

        public TypeFilter Add(ITypeFilter filter)
        {
            Filters.Add(filter);

            return this;
        }

        public IEnumerable<Type> Filter(Type type)
        {
            var results = new HashSet<Type>();
            foreach (var filter in Filters)
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

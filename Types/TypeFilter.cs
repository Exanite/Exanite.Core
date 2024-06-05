using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class TypeFilter : ITypeFilter
    {
        public List<ITypeFilter> Filters { get; } = new();

        public TypeFilter Self()
        {
            Filters.Add(TypeFilters.Self);

            return this;
        }

        public TypeFilter Interfaces()
        {
            Filters.Add(TypeFilters.Interface);

            return this;
        }

        public TypeFilter InheritanceHierarchy<TBaseType>(bool inclusive = false, bool mustExtendBaseType = false)
        {
            Filters.Add(new InheritanceHierarchyTypeFilter(typeof(TBaseType), inclusive, mustExtendBaseType));

            return this;
        }

        public TypeFilter InheritanceHierarchy(Type baseType = null, bool inclusive = false, bool mustExtendBaseType = false)
        {
            if (baseType == null && inclusive == false && mustExtendBaseType == false)
            {
                Filters.Add(TypeFilters.InheritanceHierarchy);
            }
            else
            {
                Filters.Add(new InheritanceHierarchyTypeFilter(baseType, inclusive, mustExtendBaseType));
            }

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

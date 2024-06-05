using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class TypeFilter : ITypeFilter
    {
        public List<ITypeFilter> Filters { get; } = new();

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

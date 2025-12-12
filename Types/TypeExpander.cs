using System;
using System.Collections.Generic;

namespace Exanite.Core.Types;

public class TypeExpander : ITypeExpander
{
    public IReadOnlyList<ITypeExpander> Filters { get; }

    public TypeExpander(IEnumerable<ITypeExpander> filters)
    {
        Filters = [..filters];
    }

    public IEnumerable<Type> Expand(Type type)
    {
        var results = new HashSet<Type>();
        foreach (var filter in Filters)
        {
            foreach (var filterType in filter.Expand(type))
            {
                results.Add(filterType);
            }
        }

        return results;
    }
}

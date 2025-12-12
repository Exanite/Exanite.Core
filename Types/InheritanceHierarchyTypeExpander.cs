using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Types;

public class InheritanceHierarchyTypeExpander : ITypeExpander
{
    public IReadOnlySet<Type> BaseTypes { get; }

    public bool MustExtendBaseType { get; }

    public bool IncludeInputType { get; }
    // public bool IncludeBaseType { get; } // TODO

    public InheritanceHierarchyTypeExpander(Type? baseType = null, bool includeInputType = false, bool mustExtendBaseType = false)
        : this([baseType], includeInputType, mustExtendBaseType) {}

    public InheritanceHierarchyTypeExpander(IEnumerable<Type?> baseTypes, bool includeInputType = false, bool mustExtendBaseType = false)
    {
        BaseTypes = new HashSet<Type>(baseTypes.Where(type => type != null).Cast<Type>());
        IncludeInputType = includeInputType;
        MustExtendBaseType = mustExtendBaseType;
    }

    public IEnumerable<Type> Expand(Type type)
    {
        var hasReachedBaseType = false;
        if (BaseTypes.Count == 0)
        {
            hasReachedBaseType = true;
        }

        var currentType = type;
        while (currentType != null)
        {
            if (BaseTypes.Contains(currentType))
            {
                if (IncludeInputType)
                {
                    yield return currentType;
                }

                hasReachedBaseType = true;

                break;
            }

            yield return currentType;
            currentType = currentType.BaseType;
        }

        if (MustExtendBaseType && !hasReachedBaseType)
        {
            throw new ArgumentException($"Type '{type.Name}' does not inherit from any of the specified base types", nameof(type));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Types;

/// <summary>
/// Type expander that returns the class inheritance hierarchy of the input type.
/// </summary>
/// <remarks>
/// Interfaces are not returned.
/// </remarks>
public class InheritanceHierarchyTypeExpander : ITypeExpander
{
    /// <summary>
    /// A set of types that determine when the type expander stops traversing the type inheritance hierarchy.
    /// </summary>
    public IReadOnlySet<Type> BaseTypes { get; }

    /// <summary>
    /// Whether the input type must extend from a base type specified by <see cref="BaseTypes"/>.
    /// An exception will be thrown if this is true and the input type does not extend from a base type.
    /// </summary>
    public bool MustExtendBaseType { get; init; } = false;

    /// <summary>
    /// Whether the input type is included in the results.
    /// </summary>
    public bool IncludeInputType { get; init; } = true;

    /// <summary>
    /// Whether the base type is included in the results.
    /// </summary>
    public bool IncludeBaseType { get; init; } = false;

    public InheritanceHierarchyTypeExpander(IEnumerable<Type>? baseTypes = null)
    {
        baseTypes ??= [];
        BaseTypes = baseTypes.ToHashSet();
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
                // Base type reached
                hasReachedBaseType = true;

                if (IncludeBaseType)
                {
                    yield return currentType;
                }

                break;
            }

            if (IncludeInputType || currentType != type)
            {
                yield return currentType;
            }

            currentType = currentType.BaseType;
        }

        if (MustExtendBaseType && !hasReachedBaseType)
        {
            throw new ArgumentException($"Type '{type.Name}' does not inherit from any of the specified base types", nameof(type));
        }
    }
}

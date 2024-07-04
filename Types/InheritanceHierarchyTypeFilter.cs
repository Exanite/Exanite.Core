using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public class InheritanceHierarchyTypeFilter : ITypeFilter
    {
        public HashSet<Type?> BaseTypes { get; }
        public bool Inclusive { get; set; }
        public bool MustExtendBaseType { get; set; }

        public InheritanceHierarchyTypeFilter(Type? baseType = null, bool inclusive = false, bool mustExtendBaseType = false) : this(new HashSet<Type?>() { baseType }, inclusive, mustExtendBaseType) {}

        public InheritanceHierarchyTypeFilter(IEnumerable<Type?> baseTypes, bool inclusive = false, bool mustExtendBaseType = false) : this(new HashSet<Type?>(baseTypes), inclusive, mustExtendBaseType) {}

        private InheritanceHierarchyTypeFilter(HashSet<Type?> baseTypes, bool inclusive, bool mustExtendBaseType)
        {
            BaseTypes = baseTypes;
            Inclusive = inclusive;
            MustExtendBaseType = mustExtendBaseType;

            BaseTypes.Remove(null);
        }

        public IEnumerable<Type> Filter(Type type)
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
                    if (Inclusive)
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
}

using System;
using System.Collections.Generic;

namespace Exanite.Core.Types
{
    public interface IInstanceTypeFilter
    {
        public IEnumerable<Type> Filter(Type type);
    }

    public class InheritanceHierarchyTypeFilter : IInstanceTypeFilter
    {
        public Type BaseType { get; }
        public bool Inclusive { get; }
        public bool MustExtendBaseType { get; }

        public InheritanceHierarchyTypeFilter() {}

        public InheritanceHierarchyTypeFilter(Type baseType, bool inclusive = false, bool mustExtendBaseType = false)
        {
            BaseType = baseType;
            Inclusive = inclusive;
            MustExtendBaseType = mustExtendBaseType;
        }

        public IEnumerable<Type> Filter(Type type)
        {
            var hasReachedBaseType = false;
            if (BaseType == null)
            {
                hasReachedBaseType = true;
            }

            var currentType = type;
            while (currentType != null)
            {
                if (currentType == BaseType)
                {
                    break;
                }

                yield return currentType;
                currentType = currentType.BaseType;
            }

            if (MustExtendBaseType && !hasReachedBaseType)
            {
                throw new ArgumentException($"Type '{type.Name}' does not inherit from base type '{BaseType.Name}'", nameof(type));
            }
        }
    }

    public class SelfTypeFilter : IInstanceTypeFilter
    {
        public IEnumerable<Type> Filter(Type type)
        {
            yield return type;
        }
    }

    public class InterfaceTypeFilter : IInstanceTypeFilter
    {
        public IEnumerable<Type> Filter(Type type)
        {
            return type.GetInterfaces();
        }
    }
}

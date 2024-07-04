using System;

namespace Exanite.Core.Types
{
    public static class TypeFilterUtility
    {
        public static TypeFilter Self(this TypeFilter typeFilter)
        {
            typeFilter.Add(TypeFilters.Self);

            return typeFilter;
        }

        public static TypeFilter Interfaces(this TypeFilter typeFilter)
        {
            typeFilter.Add(TypeFilters.Interface);

            return typeFilter;
        }

        public static TypeFilter InheritanceHierarchy<TBaseType>(this TypeFilter typeFilter, bool inclusive = false, bool mustExtendBaseType = false)
        {
            typeFilter.Add(new InheritanceHierarchyTypeFilter(typeof(TBaseType), inclusive, mustExtendBaseType));

            return typeFilter;
        }

        public static TypeFilter InheritanceHierarchy(this TypeFilter typeFilter, Type? baseType = null, bool inclusive = false, bool mustExtendBaseType = false)
        {
            if (baseType == null && inclusive == false && mustExtendBaseType == false)
            {
                typeFilter.Add(TypeFilters.InheritanceHierarchy);
            }
            else
            {
                typeFilter.Add(new InheritanceHierarchyTypeFilter(baseType, inclusive, mustExtendBaseType));
            }

            return typeFilter;
        }
    }
}

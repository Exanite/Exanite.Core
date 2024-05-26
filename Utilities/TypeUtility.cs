﻿using System;
using System.Collections.Generic;

namespace Exanite.Core.Utilities
{
    public static class TypeUtility
    {
        /// <summary>
        /// Returns true if the type is not abstract or generic.
        /// </summary>
        public static bool IsConcrete(this Type type)
        {
            return !(type.IsAbstract || type.IsGenericType);
        }

        /// <summary>
        /// Gets the default value for the provided <see cref="Type"/>.
        /// </summary>
        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <inheritdoc cref="GetInheritanceHierarchyUpTo"/>
        public static IEnumerable<Type> GetInheritanceHierarchyUpTo<TBaseType>(Type instanceType, bool isInclusive)
        {
            return GetInheritanceHierarchyUpTo(typeof(TBaseType), instance, isInclusive);
        }

        /// <summary>
        /// Given an instance, this method will return the types starting from the instance's type up to the specified base type.
        /// </summary>
        public static IEnumerable<Type> GetInheritanceHierarchyUpTo(Type baseType, Type instanceType, bool isInclusive)
        {
            var currentType = instanceType;
            while (currentType != baseType)
            {
                if (currentType == null)
                {
                    throw new ArgumentException($"Instance type '{instanceType.Name}' does not inherit from base type '{baseType.Name}'", nameof(instanceType));
                }

                yield return currentType;

                currentType = currentType.BaseType;
            }

            yield return baseType;
        }
    }
}

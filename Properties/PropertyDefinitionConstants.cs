using System;
using System.Collections.Generic;

namespace Exanite.Core.Properties
{
    /// <summary>
    ///     Predefined constant property definitions for ease of use.
    ///     Repeated calls of a GetConstant method will return the same
    ///     property definition. Constants will be prefixed with `PDC_`
    /// </summary>
    public static class PropertyDefinitionConstants
    {
        private static readonly string PropertyPrefix = "PDC_";

        private static readonly Dictionary<Type, PropertyDefinition> Cache = new Dictionary<Type, PropertyDefinition>();

        /// <summary>
        ///     Returns a constant property definition for the specified type.
        /// </summary>
        public static PropertyDefinition<T> GetConstantForType<T>()
        {
            if (!Cache.ContainsKey(typeof(T)))
            {
                Cache[typeof(T)] = new PropertyDefinition<T>($"{PropertyPrefix}{typeof(T).FullName}");
            }

            return Cache[typeof(T)] as PropertyDefinition<T>;
        }
    }
}
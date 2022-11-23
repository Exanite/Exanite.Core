using System;
using System.Collections.Generic;

namespace Exanite.Core.Properties
{
    /// <summary>
    ///     Stores cached property definitions.
    /// </summary>
    public class PropertyDefinitionCache
    {
        private readonly string namePrefix;
        private readonly Dictionary<Type, PropertyDefinition> cache = new();

        public PropertyDefinitionCache(string namePrefix)
        {
            this.namePrefix = namePrefix;
        }

        /// <summary>
        ///     Returns a cached property definition for the specified type.
        /// </summary>
        public PropertyDefinition<T> GetConstantForType<T>()
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                cache[typeof(T)] = new PropertyDefinition<T>($"{namePrefix}{typeof(T).FullName}");
            }

            return cache[typeof(T)] as PropertyDefinition<T>;
        }
    }
}
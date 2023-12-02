using System;
using System.Collections.Generic;

namespace Exanite.Core.Properties
{
    /// <summary>
    /// Stores cached property definitions.
    /// </summary>
    public class PropertyDefinitionCache
    {
        private readonly string keyPrefix;
        private readonly Dictionary<Type, PropertyDefinition> cache = new();

        public PropertyDefinitionCache(string keyPrefix)
        {
            this.keyPrefix = keyPrefix;
        }

        /// <summary>
        /// Returns a cached property definition for the specified type.
        /// </summary>
        public PropertyDefinition<T> GetConstantForType<T>()
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                cache[typeof(T)] = new PropertyDefinition<T>($"{keyPrefix}{typeof(T).FullName}");
            }

            return cache[typeof(T)] as PropertyDefinition<T>;
        }
    }
}

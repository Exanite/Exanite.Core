using System.Collections.Generic;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyCollectionSchemaBuilder
    {
        private readonly List<PropertyCollectionSchemaEntryBuilder> entryBuilders = new();

        public PropertyCollectionSchemaBuilder Add(PropertyCollectionSchemaEntryBuilder entryBuilder)
        {
            entryBuilders.Add(entryBuilder);

            return this;
        }

        public PropertyCollectionSchema Build()
        {
            var entries = new List<PropertyCollectionSchemaEntry>();

            foreach (var entryBuilder in entryBuilders)
            {
                entries.Add(entryBuilder.Build());
            }

            return new PropertyCollectionSchema(entries);
        }
    }
}
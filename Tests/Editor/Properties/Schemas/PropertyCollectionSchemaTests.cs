using NUnit.Framework;
using Exanite.Core.Properties;
using Exanite.Core.Properties.Schemas;

namespace Exanite.Core.Tests.Editor.Properties.Schemas
{
    [TestFixture]
    public class PropertyCollectionSchemaTests
    {
        private static readonly PropertyDefinition<string> SharedDefinition = new PropertyDefinition<string>("Shared");
        
        [Test]
        public void Validate_WhenCollectionMissingOptionalProperty_ReturnsTrue()
        {
            var schema = new PropertyCollectionSchemaBuilder()
                .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition))
                .Build();

            var collection = new PropertyCollection();
            
            Assert.IsTrue(schema.Validate(collection));
        }
        
        [Test]
        public void Validate_WhenCollectionHasOptionalProperty_ReturnsTrue()
        {
            var schema = new PropertyCollectionSchemaBuilder()
                .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition))
                .Build();

            var collection = new PropertyCollection();
            collection.GetProperty(SharedDefinition);
            
            Assert.IsTrue(schema.Validate(collection));
        }
        
        [Test]
        public void Validate_WhenCollectionMissingRequiredProperty_ReturnsFalse()
        {
            var schema = new PropertyCollectionSchemaBuilder()
                .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition)
                    .AsRequired())
                .Build();

            var collection = new PropertyCollection();
            
            Assert.IsFalse(schema.Validate(collection));
        }
        
        [Test]
        public void Validate_WhenCollectionHasRequiredProperty_ReturnsTrue()
        {
            var schema = new PropertyCollectionSchemaBuilder()
                .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition)
                    .AsRequired())
                .Build();

            var collection = new PropertyCollection();
            collection.GetProperty(SharedDefinition);
            
            Assert.IsTrue(schema.Validate(collection));
        }

        [Test]
        public void Validate_WhenCollectionHasPropertyOfWrongType_ReturnsFalse()
        {
            var sharedName = "ConflictingProperty";
            
            var existingDefinition = new PropertyDefinition<string>(sharedName);
            var schemaDefinition = new PropertyDefinition<int>(sharedName);
            var schema = new PropertyCollectionSchemaBuilder()
                .Add(new PropertyCollectionSchemaEntryBuilder(schemaDefinition))
                .Build();
            
            var collection = new PropertyCollection();
            collection.GetProperty(existingDefinition);
            
            Assert.IsFalse(schema.Validate(collection));
        }
    }
}
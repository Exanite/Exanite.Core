using Exanite.Core.Properties;
using Exanite.Core.Properties.Schemas;
using Xunit;

namespace Exanite.Core.Tests.Properties.Schemas;

public class PropertyCollectionSchemaTests
{
    private static readonly PropertyDefinition<string> SharedDefinition = new("Shared");

    [Fact]
    public void Validate_WhenCollectionMissingOptionalProperty_ReturnsTrue()
    {
        var schema = new PropertyCollectionSchemaBuilder()
            .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition)
                .AsOptional())
            .Build();

        var collection = new PropertyCollection();

        Assert.True(schema.Validate(collection));
    }

    [Fact]
    public void Validate_WhenCollectionHasOptionalProperty_ReturnsTrue()
    {
        var schema = new PropertyCollectionSchemaBuilder()
            .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition)
                .AsOptional())
            .Build();

        var collection = new PropertyCollection();
        collection.AddProperty(SharedDefinition);

        Assert.True(schema.Validate(collection));
    }

    [Fact]
    public void Validate_WhenCollectionMissingRequiredProperty_ReturnsFalse()
    {
        var schema = new PropertyCollectionSchemaBuilder()
            .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition))
            .Build();

        var collection = new PropertyCollection();

        Assert.False(schema.Validate(collection));
    }

    [Fact]
    public void Validate_WhenCollectionHasRequiredProperty_ReturnsTrue()
    {
        var schema = new PropertyCollectionSchemaBuilder()
            .Add(new PropertyCollectionSchemaEntryBuilder(SharedDefinition))
            .Build();

        var collection = new PropertyCollection();
        collection.AddProperty(SharedDefinition);

        Assert.True(schema.Validate(collection));
    }

    [Fact]
    public void Validate_WhenCollectionHasPropertyOfWrongType_ReturnsFalse()
    {
        var sharedName = "ConflictingProperty";

        var existingDefinition = new PropertyDefinition<string>(sharedName);
        var schemaDefinition = new PropertyDefinition<int>(sharedName);
        var schema = new PropertyCollectionSchemaBuilder()
            .Add(new PropertyCollectionSchemaEntryBuilder(schemaDefinition))
            .Build();

        var collection = new PropertyCollection();
        collection.AddProperty(existingDefinition);

        Assert.False(schema.Validate(collection));
    }
}

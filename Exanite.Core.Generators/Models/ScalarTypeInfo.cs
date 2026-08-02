namespace Exanite.Core.Generators.Models;

public readonly record struct ScalarTypeInfo
{
    public required ScalarType Type { get; init; }
    public required string TypeName { get; init; }
    public required string Suffix { get; init; }
}

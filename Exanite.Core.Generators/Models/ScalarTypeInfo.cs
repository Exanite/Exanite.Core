namespace Exanite.Core.Generators.Models;

public readonly record struct ScalarTypeInfo
{
    public required ScalarType Type { get; init; }
    public required string ScalarName { get; init; }
    public required string Suffix { get; init; }

    public string VectorName(int componentCount)
    {
        return $"Vector{componentCount}{Suffix}";
    }

    public string RectName(int componentCount)
    {
        return $"Rect{componentCount}{Suffix}";
    }
}

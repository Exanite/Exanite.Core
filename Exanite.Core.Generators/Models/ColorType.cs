using System.Collections.Immutable;
using Exanite.Core.Numerics;

namespace Exanite.Core.Generators.Models;

public readonly record struct ColorTypeInfo
{
    public required ColorType Type { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }

    public required ImmutableArray<string> Components { get; init; }

    public string StorageName(int componentCount)
    {
        return $"{Name}Color{componentCount}";
    }
}

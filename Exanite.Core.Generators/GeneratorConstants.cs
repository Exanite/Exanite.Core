using System.Collections.Immutable;
using Exanite.Core.Generators.Models;

namespace Exanite.Core.Generators;

public static class GeneratorConstants
{
    public static readonly ImmutableArray<string> VectorComponents = ["X", "Y", "Z", "W"];

    public static readonly ImmutableArray<ScalarTypeInfo> ScalarTypes =
    [
        new()
        {
            Type = ScalarType.Float,
            TypeName = "float",
            Suffix = "",
        },
        new()
        {
            Type = ScalarType.Fixed,
            TypeName = "fixed",
            Suffix = "Fixed",
        },
        new()
        {
            Type = ScalarType.Int,
            TypeName = "int",
            Suffix = "Int",
        },
    ];
}

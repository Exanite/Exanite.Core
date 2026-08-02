using System.Collections.Immutable;
using Exanite.Core.Generators.Models;

namespace Exanite.Core.Generators;

public static class GeneratorConstants
{
    public static readonly ImmutableArray<string> VectorComponents = ["X", "Y", "Z", "W"];

    public class Scalars
    {
        public static readonly ScalarTypeInfo Float = new()
        {
            Type = ScalarType.Float,
            ScalarName = "float",
            Suffix = "",
        };

        public static readonly ScalarTypeInfo Fixed = new()
        {
            Type = ScalarType.Fixed,
            ScalarName = "Fixed",
            Suffix = "Fixed",
        };

        public static readonly ScalarTypeInfo Int = new()
        {
            Type = ScalarType.Int,
            ScalarName = "int",
            Suffix = "Int",
        };

        public static readonly ImmutableArray<ScalarTypeInfo> Types =
        [
            Float,
            Fixed,
            Int,
        ];

        public static string? CastType(ScalarTypeInfo from, ScalarTypeInfo to)
        {
            return (from.Type, to.Type) switch
            {
                (ScalarType.Float, ScalarType.Fixed) => "explicit",
                (ScalarType.Fixed, ScalarType.Float) => "explicit",

                (ScalarType.Int, ScalarType.Fixed) => "implicit",
                (ScalarType.Fixed, ScalarType.Int) => "explicit",

                (ScalarType.Float, ScalarType.Int) => "explicit",
                (ScalarType.Int, ScalarType.Float) => "implicit",

                _ => null,
            };
        }
    }
}

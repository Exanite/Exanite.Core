using System.Collections.Immutable;
using Exanite.CodeGen;
using Exanite.Core.Io;
using static Exanite.Core.Generators.GeneratorConstants;

namespace Exanite.Core.Generators.Generators;

public class ColorStorageGenerator
{
    private static readonly ImmutableArray<ColorType> ColorTypes =
    [
        new()
        {
            Name = "Srgb",
            DisplayName = "sRGB",
        },
        new()
        {
            Name = "Linear",
            DisplayName = "Linear",
        },
        new()
        {
            Name = "Hsl",
            DisplayName = "HSL",
        },
    ];

    public void Run()
    {
        foreach (var currentType in ColorTypes)
        {
            for (var componentCount = 3; componentCount <= 4; componentCount++)
            {
                var storageName = currentType.StorageName(componentCount);
                var vectorName = Scalars.Float.VectorName(componentCount);

                var builder = new IndentedStringBuilder();
                builder.AppendGeneratedCodeHeader();

                builder.AppendLine("using System.Numerics;");
                builder.AppendLine("using System.Runtime.InteropServices;");
                builder.AppendLine("using Exanite.Core.Utilities;");
                builder.AppendLine();
                builder.AppendLine("namespace Exanite.Core.Numerics;");

                builder.AppendSeparation();
                builder.AppendBlock($"""
                    /// <summary>
                    /// Storage struct for explicitly storing a color in {currentType.DisplayName} format.
                    /// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in {currentType.DisplayName}.
                    /// <br/>
                    /// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
                    /// </summary>
                    """);
                builder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
                using (builder.EnterScope($"public partial record struct {storageName}"))
                {
                    builder.AppendSeparation();
                    builder.AppendLine($"public {vectorName} Value;");
                    builder.AppendLine("public readonly Color Color => this;");

                    builder.AppendSeparation();
                    using (builder.EnterScope($"public {storageName}({vectorName} value)"))
                    {
                        builder.AppendLine("Value = value;");
                    }

                    builder.AppendSeparation();
                    using (builder.EnterScope($"public static implicit operator Color({storageName} color)"))
                    {
                        builder.AppendLine($"return Color.From{currentType.Name}(color.Value);");
                    }

                    builder.AppendSeparation();
                    using (builder.EnterScope($"public static implicit operator {storageName}(Color color)"))
                    {
                        if (componentCount == 3)
                        {
                            builder.AppendLine($"return new {storageName}(color.{currentType.Name}.Value.Xyz());");
                        }
                        else
                        {
                            builder.AppendLine($"return new {storageName}(color.{currentType.Name}.Value);");
                        }
                    }

                    builder.AppendSeparation();
                    using (builder.EnterScope($"public static implicit operator {storageName}({vectorName} color)"))
                    {
                        builder.AppendLine($"return new {storageName}(color);");
                    }

                    builder.AppendSeparation();
                    using (builder.EnterScope($"public static implicit operator {vectorName}({storageName} color)"))
                    {
                        builder.AppendLine("return color.Value;");
                    }

                    builder.AppendSeparation();
                    using (builder.EnterScope("public readonly override string ToString()"))
                    {
                        builder.AppendLine("return Color.ToString();");
                    }
                }

                var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{storageName}.g.cs";
                outputPath.WriteAllText(builder.ToString());
            }
        }
    }

    private readonly record struct ColorType
    {
        public required string Name { get; init; }

        public required string DisplayName { get; init; }

        public string StorageName(int componentCount)
        {
            return $"{Name}Color{componentCount}";
        }
    }
}

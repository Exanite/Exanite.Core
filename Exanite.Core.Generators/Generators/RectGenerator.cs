using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generators.Generators;

public class RectGenerator
{
    public void Run()
    {
        for (var componentCount = 2; componentCount <= 3; componentCount++)
        {
            var components = GeneratorConstants.VectorComponents.Take(componentCount).ToArray();

            var rectType = $"Rect{componentCount}";
            var vectorType = $"Vector{componentCount}";

            var builder = new IndentedStringBuilder();
            builder.AppendGeneratedCodeHeader();

            builder.AppendLine("using System.Numerics;");
            builder.AppendLine();
            builder.AppendLine("namespace Exanite.Core.Numerics;");

            builder.AppendSeparation();
            using (builder.EnterScope($"public partial record struct {rectType}"))
            {
                builder.AppendSeparation();
                builder.AppendLine($"public static readonly {rectType} Zero = default;");
                builder.AppendLine($"public static readonly {rectType} One = FromSize({vectorType}.One);");

                builder.AppendSeparation();
                builder.AppendLine($"public {vectorType} Offset;");
                builder.AppendLine($"public {vectorType} Size;");

                builder.AppendSeparation();
                using (builder.EnterScope($"public static {rectType} FromSize({vectorType} size)"))
                {
                    builder.AppendLine($"return new {rectType}()");
                    using (builder.Indent("{"))
                    {
                        builder.AppendLine("Size = size,");
                    }
                    builder.AppendLine("};");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public static {rectType} FromOffsetSize({vectorType} offset, {vectorType} size)"))
                {
                    builder.AppendLine($"return new {rectType}()");
                    using (builder.Indent("{"))
                    {
                        builder.AppendLine("Offset = offset,");
                        builder.AppendLine("Size = size,");
                    }
                    builder.AppendLine("};");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public readonly {rectType} Scale({vectorType} scale)"))
                {
                    builder.AppendLine("return FromOffsetSize(scale * Offset, scale * Size);");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public readonly bool Contains({vectorType} position)"))
                {
                    var checks = components.Select(c => $"position.{c} >= Offset.{c} && position.{c} < Offset.{c} + Size.{c}");
                    builder.AppendBlock($"return {string.Join("\n    && ", checks)};");
                }
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{rectType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

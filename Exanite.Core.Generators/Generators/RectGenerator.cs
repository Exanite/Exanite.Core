using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;
using static Exanite.Core.Generators.GeneratorConstants;

namespace Exanite.Core.Generators.Generators;

public class RectGenerator
{
    public void Run()
    {
        foreach (var currentType in Scalars.Types)
        {
            for (var componentCount = 2; componentCount <= 3; componentCount++)
            {
                var components = VectorComponents.Take(componentCount).ToArray();

                var rectType = currentType.RectName(componentCount);
                var vectorType = currentType.VectorName(componentCount);

                var builder = new IndentedStringBuilder();
                builder.AppendGeneratedCodeHeader();

                builder.AppendLine("using System.Numerics;");
                builder.AppendLine();
                builder.AppendLine("namespace Exanite.Core.Numerics;");

                builder.AppendSeparation();
                using (builder.EnterScope($"public partial record struct {rectType}"))
                {
                    AppendConstants(builder, rectType, vectorType);
                    AppendFields(builder, vectorType);

                    // Cast to other
                    foreach (var otherType in Scalars.Types)
                    {
                        var castType = Scalars.CastType(currentType, otherType);
                        if (castType != null)
                        {
                            AppendCastOperation(builder, castType, rectType, otherType.RectName(componentCount), otherType.VectorName(componentCount));
                        }
                    }

                    AppendCreateOperations(builder, rectType, vectorType);
                    AppendScaleOperation(builder, rectType, vectorType);
                    AppendContainsOperation(builder, vectorType, components);
                }

                var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{rectType}.g.cs";
                outputPath.WriteAllText(builder.ToString());
            }
        }
    }

    private static void AppendConstants(IndentedStringBuilder builder, string rectType, string vectorType)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public static readonly {rectType} Zero = default;");
        builder.AppendLine($"public static readonly {rectType} One = FromSize({vectorType}.One);");
    }

    private static void AppendFields(IndentedStringBuilder builder, string vectorType)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public {vectorType} Offset;");
        builder.AppendLine($"public {vectorType} Size;");
    }

    private static void AppendCastOperation(IndentedStringBuilder builder, string castType, string srcRectType, string dstRectType, string dstVectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {castType} operator {dstRectType}({srcRectType} value)"))
        {
            builder.AppendLine($"return {dstRectType}.FromOffsetSize(({dstVectorType})value.Offset, ({dstVectorType})value.Size);");
        }
    }

    private static void AppendCreateOperations(IndentedStringBuilder builder, string rectType, string vectorType)
    {
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
    }

    private static void AppendScaleOperation(IndentedStringBuilder builder, string rectType, string vectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public readonly {rectType} Scale({vectorType} scale)"))
        {
            builder.AppendLine("return FromOffsetSize(scale * Offset, scale * Size);");
        }
    }

    private static void AppendContainsOperation(IndentedStringBuilder builder, string vectorType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public readonly bool Contains({vectorType} position)"))
        {
            var checks = components.Select(c => $"position.{c} >= Offset.{c} && position.{c} < Offset.{c} + Size.{c}");
            builder.AppendBlock($"return {string.Join("\n    && ", checks)};");
        }
    }
}

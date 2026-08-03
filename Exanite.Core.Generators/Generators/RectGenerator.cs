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
                var rectName = currentType.RectName(componentCount);
                var vectorName = currentType.VectorName(componentCount);

                var builder = new IndentedStringBuilder();
                builder.AppendGeneratedCodeHeader();

                builder.AppendLine("using System.Numerics;");
                builder.AppendLine();
                builder.AppendLine("namespace Exanite.Core.Numerics;");

                builder.AppendSeparation();
                using (builder.EnterScope($"public partial record struct {rectName}"))
                {
                    AppendConstants(builder, rectName, vectorName);
                    AppendFields(builder, vectorName);

                    // Cast from other
                    foreach (var otherType in Scalars.Types)
                    {
                        var castType = Scalars.CastType(otherType, currentType);
                        if (castType != null)
                        {
                            AppendCastOperation(builder, castType, otherType.RectName(componentCount), rectName, vectorName);
                        }
                    }

                    AppendCreateOperations(builder, rectName, vectorName);
                    AppendScaleOperation(builder, rectName, vectorName);
                    AppendContainsOperation(builder, vectorName, components);
                    AppendIntersectsOperation(builder, rectName, components);
                }

                var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{rectName}.g.cs";
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
            builder.AppendLine($"return FromOffsetSize(({dstVectorType})value.Offset, ({dstVectorType})value.Size);");
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

    private static void AppendIntersectsOperation(IndentedStringBuilder builder, string rectType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public readonly bool Intersects({rectType} other)"))
        {
            var checks = components.Select(c => $"Offset.{c} < other.Offset.{c} + other.Size.{c} && Offset.{c} + Size.{c} > other.Offset.{c}");
            builder.AppendBlock($"return {string.Join("\n    && ", checks)};");
        }
    }
}

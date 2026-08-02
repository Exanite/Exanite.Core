using System.Linq;
using Exanite.CodeGen;

namespace Exanite.Core.Generators.Generators;

public abstract class RectGenerator
{
    protected void AppendConstants(IndentedStringBuilder builder, string rectType, string vectorType)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public static readonly {rectType} Zero = default;");
        builder.AppendLine($"public static readonly {rectType} One = FromSize({vectorType}.One);");
    }

    protected void AppendFields(IndentedStringBuilder builder, string vectorType)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public {vectorType} Offset;");
        builder.AppendLine($"public {vectorType} Size;");
    }

    protected void AppendRectCastOperation(IndentedStringBuilder builder, string castType, string srcRectType, string dstRectType, string dstVectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {castType} operator {dstRectType}({srcRectType} value)"))
        {
            builder.AppendLine($"return {dstRectType}.FromOffsetSize(({dstVectorType})value.Offset, ({dstVectorType})value.Size);");
        }
    }

    protected void AppendCreateOperations(IndentedStringBuilder builder, string rectType, string vectorType)
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

    protected void AppendScaleOperation(IndentedStringBuilder builder, string rectType, string vectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public readonly {rectType} Scale({vectorType} scale)"))
        {
            builder.AppendLine("return FromOffsetSize(scale * Offset, scale * Size);");
        }
    }

    protected void AppendContainsOperation(IndentedStringBuilder builder, string vectorType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public readonly bool Contains({vectorType} position)"))
        {
            var checks = components.Select(c => $"position.{c} >= Offset.{c} && position.{c} < Offset.{c} + Size.{c}");
            builder.AppendBlock($"return {string.Join("\n    && ", checks)};");
        }
    }
}

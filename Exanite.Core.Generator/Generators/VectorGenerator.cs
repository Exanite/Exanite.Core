using System.Linq;
using Exanite.CodeGen;

namespace Exanite.Core.Generator.Generators;

public class VectorGenerator
{
    protected void AppendComponentFields(IndentedStringBuilder builder, string backingType, string[] components)
    {
        foreach (var component in components)
        {
            builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.{component}\"/>");
            builder.AppendLine($"public {backingType} {component};");
            builder.AppendLine();
        }
    }

    protected void AppendIdentityVectorConstants(IndentedStringBuilder builder, string selfVectorType, string[] components)
    {
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Zero\"/>");
        builder.AppendLine($"public static {selfVectorType} Zero => default;");
        builder.AppendLine();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.One\"/>");
        builder.AppendLine($"public static {selfVectorType} One => new(1);");
    }

    protected void AppendBasisVectorConstants(IndentedStringBuilder builder, string selfVectorType, string[] components)
    {
        for (var i = 0; i < components.Length; i++)
        {
            var currentComponent = i;
            var parameters = string.Join(", ", Enumerable.Range(0, components.Length).Select(index => index == currentComponent ? "1" : "0"));

            builder.AppendSeparation();
            builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Unit{components[i]}\"/>");
            builder.AppendLine($"public static {selfVectorType} Unit{components[i]} => new({parameters});");
        }
    }

    protected void AppendIndexer(IndentedStringBuilder builder, string backingType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public {backingType} this[int index]"))
        {
            using (builder.EnterScope("readonly get"))
            {
                using (builder.EnterScope("switch (index)"))
                {
                    for (var i = 0; i < components.Length; i++)
                    {
                        builder.AppendLine($"case {i}: return {components[i]};");
                    }
                    builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                }
            }

            builder.AppendSeparation();
            using (builder.EnterScope("set"))
            {
                using (builder.EnterScope("switch (index)"))
                {
                    for (var i = 0; i < components.Length; i++)
                    {
                        builder.AppendLine($"case {i}: {components[i]} = value; break;");
                    }
                    builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                }
            }
        }
    }

    protected void AppendConstructors(IndentedStringBuilder builder, string selfVectorType, string backingType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public {selfVectorType}({backingType} value) : this({string.Join(", ", components.Select(_ => "value"))}) {{}}");

        builder.AppendSeparation();
        using (builder.EnterScope($"public {selfVectorType}({string.Join(", ", components.Select(c => $"{backingType} {c.ToLower()}"))})"))
        {
            foreach (var component in components)
            {
                builder.AppendLine($"{component} = {component.ToLower()};");
            }
        }
    }

    protected void AppendVectorCastOperation(IndentedStringBuilder builder, string castType, string srcVectorType, string dstVectorType, string dstBackingType, string[] components, bool manualSeparation = false)
    {
        // VectorFixedGenerator adds some comments to these operations, so it handles the separation manually
        if (!manualSeparation)
        {
            builder.AppendSeparation();
        }

        using (builder.EnterScope($"public static {castType} operator {dstVectorType}({srcVectorType} value)"))
        {
            builder.AppendLine($"return new {dstVectorType}({string.Join(", ", components.Select(c => $"({dstBackingType})value.{c}"))});");
        }
    }

    protected void AppendScalarOperation(IndentedStringBuilder builder, string[] components, string leftInputType, string rightInputType, string returnType, string operation)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {returnType} operator {operation}({leftInputType} value, {rightInputType} scalar)"))
        {
            builder.AppendLine($"return new {returnType}({string.Join(", ", components.Select(c => $"value.{c} {operation} scalar"))});");
        }
    }

    protected void AppendVectorOperation(IndentedStringBuilder builder, string[] components, string leftInputType, string rightInputType, string returnType, string operation)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {returnType} operator {operation}({leftInputType} left, {rightInputType} right)"))
        {
            builder.AppendLine($"return new {returnType}({string.Join(", ", components.Select(c => $"left.{c} {operation} right.{c}"))});");
        }
    }

    protected void AppendNegateOperation(IndentedStringBuilder builder, string selfVectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {selfVectorType} operator -({selfVectorType} value)"))
        {
            builder.AppendLine("return Zero - value;");
        }
    }

    protected void AppendLengthOperation(IndentedStringBuilder builder, string selfVectorType, string backingType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {backingType} Length({selfVectorType} value)"))
        {
            builder.AppendLine($"return {backingType}.Hypot({string.Join(", ", components.Select(c => $"value.{c}"))});");
        }
    }

    protected void AppendNormalizeOperation(IndentedStringBuilder builder, string selfVectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {selfVectorType} Normalize({selfVectorType} value)"))
        {
            builder.AppendLine($"return value / {selfVectorType}.Length(value);");
        }
    }

    /// <remarks>
    /// Currently designed only for self equality.
    /// </remarks>
    protected void AppendEqualityOperations(IndentedStringBuilder builder, string selfVectorType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static bool operator ==({selfVectorType} left, {selfVectorType} right)"))
        {
            builder.AppendLine("return left.Equals(right);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope($"public static bool operator !=({selfVectorType} left, {selfVectorType} right)"))
        {
            builder.AppendLine("return !left.Equals(right);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope($"public bool Equals({selfVectorType} other)"))
        {
            builder.AppendLine($"return {string.Join(" && ", components.Select(c => $"{c} == other.{c}"))};");
        }

        builder.AppendSeparation();
        using (builder.EnterScope("public override bool Equals(object? obj)"))
        {
            builder.AppendLine($"return obj is {selfVectorType} other && Equals(other);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope("public override int GetHashCode()"))
        {
            builder.AppendLine($"return HashCode.Combine({string.Join(", ", components.Select(c => $"{c}"))});");
        }
    }

    protected void AppendFormattingOperations(IndentedStringBuilder builder, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope("public override string ToString()"))
        {
            builder.AppendLine("return ToString(\"G\", CultureInfo.CurrentCulture);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope("public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)"))
        {
            builder.AppendLine("return ToString(format, CultureInfo.CurrentCulture);");
        }

        // This matches the System.Numerics Vector.ToString() implementation
        builder.AppendSeparation();
        using (builder.EnterScope("public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)"))
        {
            builder.AppendLine("string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;");
            builder.AppendLine();

            var format = string.Join("{separator} ", components.Select(c => $"{{{c}.ToString(format, formatProvider)}}"));
            builder.AppendLine($"return $\"<{format}>\";");
        }
    }
}

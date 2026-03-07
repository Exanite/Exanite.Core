using System.Linq;
using Exanite.CodeGen;

namespace Exanite.Core.Generator;

public class VectorGenerator
{
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

    /// <remarks>
    /// Currently designed only for self equality.
    /// </remarks>
    protected void AppendEqualityOperations(IndentedStringBuilder builder, string selfType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static bool operator ==({selfType} left, {selfType} right)"))
        {
            builder.AppendLine("return left.Equals(right);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope($"public static bool operator !=({selfType} left, {selfType} right)"))
        {
            builder.AppendLine("return !left.Equals(right);");
        }

        builder.AppendSeparation();
        using (builder.EnterScope($"public bool Equals({selfType} other)"))
        {
            builder.AppendLine($"return {string.Join(" && ", components.Select(c => $"{c} == other.{c}"))};");
        }

        builder.AppendSeparation();
        using (builder.EnterScope("public override bool Equals(object? obj)"))
        {
            builder.AppendLine($"return obj is {selfType} other && Equals(other);");
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

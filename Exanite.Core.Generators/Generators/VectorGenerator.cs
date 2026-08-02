using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Generators.Models;
using Exanite.Core.Io;

namespace Exanite.Core.Generators.Generators;

public class VectorGenerator
{
    public void Run()
    {
        foreach (var currentType in GeneratorConstants.Scalars.Types)
        {
            if (currentType.Type == ScalarType.Float)
            {
                continue;
            }

            for (var componentCount = 2; componentCount <= GeneratorConstants.VectorComponents.Length; componentCount++)
            {
                var components = GeneratorConstants.VectorComponents.Take(componentCount).ToArray();

                var vectorType = currentType.VectorName(componentCount);

                var builder = new IndentedStringBuilder();
                builder.AppendGeneratedCodeHeader();

                builder.AppendLine("using System;");
                builder.AppendLine("using System.Diagnostics.CodeAnalysis;");
                builder.AppendLine("using System.Globalization;");
                builder.AppendLine("using System.Numerics;");
                builder.AppendLine("using System.Runtime.InteropServices;");
                builder.AppendLine();
                builder.AppendLine("namespace Exanite.Core.Numerics;");

                builder.AppendSeparation();
                using (builder.EnterScope($"public partial struct {vectorType} : IEquatable<{vectorType}>, IFormattable"))
                {
                    AppendComponentFields(builder, currentType.ScalarName, components);

                    AppendIdentityVectorConstants(builder, vectorType, components);
                    AppendBasisVectorConstants(builder, vectorType, components);

                    AppendIndexer(builder, currentType.ScalarName, components);

                    AppendConstructors(builder, vectorType, currentType.ScalarName, components);

                    // TODO: Refactor
                    if (currentType.Type == ScalarType.Fixed)
                    {
                        var vectorFixedType = $"Vector{componentCount}Fixed";
                        var vectorIntType = $"Vector{componentCount}Int";
                        var vectorFloatType = $"Vector{componentCount}";

                        var fixedType = "Fixed";
                        var intType = "int";
                        var floatType = "float";

                        builder.AppendSeparation();
                        builder.AppendLine("// Conversion: Safe - No precision loss possible");
                        AppendVectorCastOperation(builder, "implicit", vectorIntType, vectorFixedType, fixedType, components, true);

                        builder.AppendSeparation();
                        builder.AppendLine("// Conversion: Unsafe - Non-deterministic");
                        builder.AppendLine("// Consider using Fixed.FromParts or Fixed.FromFraction instead");
                        AppendVectorCastOperation(builder, "explicit", vectorFloatType, vectorFixedType, fixedType, components, true);

                        builder.AppendSeparation();
                        builder.AppendLine("// Conversion: Loss of fraction");
                        AppendVectorCastOperation(builder, "explicit", vectorFixedType, vectorIntType, intType, components, true);

                        builder.AppendSeparation();
                        builder.AppendLine("// Conversion: Loss of precision / determinism");
                        AppendVectorCastOperation(builder, "explicit", vectorFixedType, vectorFloatType, floatType, components, true);
                    }

                    if (currentType.Type == ScalarType.Int)
                    {
                        var vectorIntType = $"Vector{componentCount}Int";
                        var vectorFloatType = $"Vector{componentCount}";

                        var intType = "int";
                        var floatType = "float";

                        AppendVectorCastOperation(builder, "explicit", vectorFloatType, vectorIntType, intType, components);
                        AppendVectorCastOperation(builder, "implicit", vectorIntType, vectorFloatType, floatType, components);
                    }

                    AppendScalarOperation(builder, components, vectorType, currentType.ScalarName, vectorType, "*");
                    if (currentType.Type == ScalarType.Int)
                    {
                        AppendScalarOperation(builder, components, vectorType, "float", GeneratorConstants.Scalars.Float.VectorName(componentCount), "*");
                    }

                    AppendScalarOperation(builder, components, vectorType, currentType.ScalarName, vectorType, "/");
                    if (currentType.Type == ScalarType.Int)
                    {
                        AppendScalarOperation(builder, components, vectorType, "float", GeneratorConstants.Scalars.Float.VectorName(componentCount), "/");
                    }

                    AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "+");
                    AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "-");
                    AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "*");
                    AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "/");
                    AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "%");

                    if (currentType.Type == ScalarType.Int)
                    {
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "<<");
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, ">>");
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, ">>>");
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "&");
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "|");
                        AppendVectorOperation(builder, components, vectorType, vectorType, vectorType, "^");
                    }

                    AppendNegateOperation(builder, vectorType);

                    if (currentType.Type == ScalarType.Fixed)
                    {
                        AppendLengthOperation(builder, vectorType, currentType.ScalarName, components);
                        AppendNormalizeOperation(builder, vectorType, components);
                    }

                    AppendDotOperation(builder, vectorType, currentType.ScalarName, components);
                    AppendCrossOperation(builder, vectorType, currentType.ScalarName, components);

                    AppendEqualityOperations(builder, vectorType, components);
                    AppendFormattingOperations(builder, components);
                }

                var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{vectorType}.g.cs";
                outputPath.WriteAllText(builder.ToString());
            }
        }
    }

    private static void AppendComponentFields(IndentedStringBuilder builder, string scalarType, string[] components)
    {
        foreach (var component in components)
        {
            builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.{component}\"/>");
            builder.AppendLine($"public {scalarType} {component};");
            builder.AppendLine();
        }
    }

    private static void AppendIdentityVectorConstants(IndentedStringBuilder builder, string selfVectorType, string[] components)
    {
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Zero\"/>");
        builder.AppendLine($"public static {selfVectorType} Zero => default;");
        builder.AppendLine();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.One\"/>");
        builder.AppendLine($"public static {selfVectorType} One => new(1);");
    }

    private static void AppendBasisVectorConstants(IndentedStringBuilder builder, string selfVectorType, string[] components)
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

    private static void AppendIndexer(IndentedStringBuilder builder, string scalarType, string[] components)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public {scalarType} this[int index]"))
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

    private static void AppendConstructors(IndentedStringBuilder builder, string selfVectorType, string scalarType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"public {selfVectorType}({scalarType} value) : this({string.Join(", ", components.Select(_ => "value"))}) {{}}");

        builder.AppendSeparation();
        using (builder.EnterScope($"public {selfVectorType}({string.Join(", ", components.Select(c => $"{scalarType} {c.ToLower()}"))})"))
        {
            foreach (var component in components)
            {
                builder.AppendLine($"{component} = {component.ToLower()};");
            }
        }
    }

    private static void AppendVectorCastOperation(IndentedStringBuilder builder, string castType, string srcVectorType, string dstVectorType, string dstScalarType, string[] components, bool manualSeparation = false)
    {
        // VectorFixedGenerator adds some comments to these operations, so it handles the separation manually
        if (!manualSeparation)
        {
            builder.AppendSeparation();
        }

        using (builder.EnterScope($"public static {castType} operator {dstVectorType}({srcVectorType} value)"))
        {
            builder.AppendLine($"return new {dstVectorType}({string.Join(", ", components.Select(c => $"({dstScalarType})value.{c}"))});");
        }
    }

    private static void AppendScalarOperation(IndentedStringBuilder builder, string[] components, string leftInputType, string rightInputType, string returnType, string operation)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {returnType} operator {operation}({leftInputType} value, {rightInputType} scalar)"))
        {
            builder.AppendLine($"return new {returnType}({string.Join(", ", components.Select(c => $"value.{c} {operation} scalar"))});");
        }
    }

    private static void AppendVectorOperation(IndentedStringBuilder builder, string[] components, string leftInputType, string rightInputType, string returnType, string operation)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {returnType} operator {operation}({leftInputType} left, {rightInputType} right)"))
        {
            builder.AppendLine($"return new {returnType}({string.Join(", ", components.Select(c => $"left.{c} {operation} right.{c}"))});");
        }
    }

    private static void AppendNegateOperation(IndentedStringBuilder builder, string selfVectorType)
    {
        builder.AppendSeparation();
        using (builder.EnterScope($"public static {selfVectorType} operator -({selfVectorType} value)"))
        {
            builder.AppendLine("return Zero - value;");
        }
    }

    private static void AppendLengthOperation(IndentedStringBuilder builder, string selfVectorType, string scalarType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Length\"/>");
        using (builder.EnterScope($"public static {scalarType} Length({selfVectorType} value)"))
        {
            builder.AppendLine($"return {scalarType}.Hypot({string.Join(", ", components.Select(c => $"value.{c}"))});");
        }
    }

    private static void AppendNormalizeOperation(IndentedStringBuilder builder, string selfVectorType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Normalize\"/>");
        using (builder.EnterScope($"public static {selfVectorType} Normalize({selfVectorType} value)"))
        {
            builder.AppendLine($"return value / {selfVectorType}.Length(value);");
        }
    }

    private static void AppendDotOperation(IndentedStringBuilder builder, string selfVectorType, string scalarType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Dot\"/>");
        using (builder.EnterScope($"public static {scalarType} Dot({selfVectorType} left, {selfVectorType} right)"))
        {
            builder.AppendLine($"return {string.Join(" + ", components.Select(c => $"left.{c} * right.{c}"))};");
        }
    }

    private static void AppendCrossOperation(IndentedStringBuilder builder, string selfVectorType, string scalarType, string[] components)
    {
        builder.AppendSeparation();
        builder.AppendLine($"/// <inheritdoc cref=\"Vector{components.Length}.Cross\"/>");
        switch (components.Length)
        {
            case 2:
            {
                using (builder.EnterScope($"public static {scalarType} Cross({selfVectorType} left, {selfVectorType} right)"))
                {
                    builder.AppendLine($"return left.{components[0]} * right.{components[1]} - left.{components[1]} * right.{components[0]};");
                }

                break;
            }
            case 3:
            {
                using (builder.EnterScope($"public static {selfVectorType} Cross({selfVectorType} left, {selfVectorType} right)"))
                {
                    using (builder.Indent($"return new {selfVectorType}("))
                    {
                        builder.AppendLine($"(left.{components[1]} * right.{components[2]}) - (left.{components[2]} * right.{components[1]}),");
                        builder.AppendLine($"(left.{components[2]} * right.{components[0]}) - (left.{components[0]} * right.{components[2]}),");
                        builder.AppendLine($"(left.{components[0]} * right.{components[1]}) - (left.{components[1]} * right.{components[0]})");
                    }
                    builder.AppendLine(");");
                }

                break;
            }
            case 4:
            {
                using (builder.EnterScope($"public static {selfVectorType} Cross({selfVectorType} left, {selfVectorType} right)"))
                {
                    using (builder.Indent($"return new {selfVectorType}("))
                    {
                        builder.AppendLine($"(left.{components[1]} * right.{components[2]}) - (left.{components[2]} * right.{components[1]}),");
                        builder.AppendLine($"(left.{components[2]} * right.{components[0]}) - (left.{components[0]} * right.{components[2]}),");
                        builder.AppendLine($"(left.{components[0]} * right.{components[1]}) - (left.{components[1]} * right.{components[0]}),");
                        builder.AppendLine($"left.{components[3]} * right.{components[3]}");
                    }
                    builder.AppendLine(");");
                }

                break;
            }
        }
    }

    /// <remarks>
    /// Currently designed only for self equality.
    /// </remarks>
    private static void AppendEqualityOperations(IndentedStringBuilder builder, string selfVectorType, string[] components)
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

    private static void AppendFormattingOperations(IndentedStringBuilder builder, string[] components)
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

using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class VectorIntGenerator
{
    private static readonly string[] AllComponents = ["X", "Y", "Z", "W"];

    public void Run()
    {
        var builder = new IndentedStringBuilder();

        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Diagnostics.CodeAnalysis;");
        builder.AppendLine("using System.Globalization;");
        builder.AppendLine("using System.Numerics;");
        builder.AppendLine("using System.Runtime.InteropServices;");
        builder.AppendLine();
        builder.AppendLine("namespace Exanite.Core.Numerics;");

        for (var componentCount = 2; componentCount <= AllComponents.Length; componentCount++)
        {
            var components = AllComponents.Take(componentCount).ToArray();
            var vectorIntType = $"Vector{componentCount}Int";
            var vectorFloatType = $"Vector{componentCount}";

            builder.Separate();
            builder.AppendLine($"public struct {vectorIntType} : IEquatable<{vectorIntType}>, IFormattable");
            using (builder.EnterScope())
            {
                for (var i = 0; i < componentCount; i++)
                {
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.{AllComponents[i]}\"/>");
                    builder.AppendLine($"public int {AllComponents[i]};");
                    builder.AppendLine();
                }

                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Zero\"/>");
                builder.AppendLine($"public static {vectorIntType} Zero => default;");
                builder.Separate();
                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.One\"/>");
                builder.AppendLine($"public static {vectorIntType} One => new(1);");

                for (var i = 0; i < componentCount; i++)
                {
                    var currentComponent = i;
                    var parameters = string.Join(", ", Enumerable.Range(0, componentCount).Select(index => index == currentComponent ? "1" : "0"));
                    builder.Separate();
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Unit{AllComponents[i]}\"/>");
                    builder.AppendLine($"public static {vectorIntType} Unit{AllComponents[i]} => new({parameters});");
                }

                builder.Separate();
                builder.AppendLine("public int this[int index]");
                using (builder.EnterScope())
                {
                    builder.AppendLine("readonly get");
                    using (builder.EnterScope())
                    {
                        builder.AppendLine("switch (index)");
                        using (builder.EnterScope())
                        {
                            for (var i = 0; i < componentCount; i++)
                            {
                                builder.AppendLine($"case {i}: return {AllComponents[i]};");
                            }
                            builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                        }
                    }

                    builder.Separate();
                    builder.AppendLine("set");
                    using (builder.EnterScope())
                    {
                        builder.AppendLine("switch (index)");
                        using (builder.EnterScope())
                        {
                            for (var i = 0; i < componentCount; i++)
                            {
                                builder.AppendLine($"case {i}: {AllComponents[i]} = value; break;");
                            }
                            builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                        }
                    }
                }

                builder.Separate();
                builder.AppendLine($"public {vectorIntType}(int value) : this({string.Join(", ", Enumerable.Range(0, componentCount).Select(_ => "value"))}) {{}}");

                builder.Separate();
                builder.AppendLine($"public {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"int {AllComponents[index].ToLower()}"))})");
                using (builder.EnterScope())
                {
                    for (var i = 0; i < componentCount; i++)
                    {
                        builder.AppendLine($"{AllComponents[i]} = {AllComponents[i].ToLower()};");
                    }
                }

                builder.Separate();
                builder.AppendLine($"public static explicit operator {vectorIntType}({vectorFloatType} value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"(int)value.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static implicit operator {vectorFloatType}({vectorIntType} value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorFloatType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator *({vectorIntType} value, int scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{AllComponents[index]} * scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorFloatType} operator *({vectorIntType} value, float scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorFloatType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{AllComponents[index]} * scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator /({vectorIntType} value, int scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{AllComponents[index]} / scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorFloatType} operator /({vectorIntType} value, float scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorFloatType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{AllComponents[index]} / scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator +({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} + right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator -({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} - right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator *({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} * right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator /({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} / right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator <<({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} << right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator >>({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} >> right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator >>>({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} >>> right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator &({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} & right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator |({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} | right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator ^({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{AllComponents[index]} ^ right.{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static {vectorIntType} operator -({vectorIntType} value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return Zero - value;");
                }

                builder.Separate();
                builder.AppendLine($"public static bool operator ==({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return left.Equals(right);");
                }

                builder.Separate();
                builder.AppendLine($"public static bool operator !=({vectorIntType} left, {vectorIntType} right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return !left.Equals(right);");
                }

                builder.Separate();
                builder.AppendLine($"public bool Equals({vectorIntType} other)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return {string.Join(" && ", Enumerable.Range(0, componentCount).Select(index => $"{AllComponents[index]} == other.{AllComponents[index]}"))};");
                }

                builder.Separate();
                builder.AppendLine("public override bool Equals(object? obj)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return obj is {vectorIntType} other && Equals(other);");
                }

                builder.Separate();
                builder.AppendLine("public override int GetHashCode()");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return HashCode.Combine({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"{AllComponents[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine("public override string ToString()");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return ToString(\"G\", CultureInfo.CurrentCulture);");
                }

                builder.Separate();
                builder.AppendLine("public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return ToString(format, CultureInfo.CurrentCulture);");
                }

                builder.Separate();
                builder.AppendLine("public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return (({vectorFloatType})this).ToString(format, formatProvider);");
                }
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "VectorXInt.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

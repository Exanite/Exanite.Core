using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class VectorIntGenerator
{
    private static readonly string[] Components = ["X", "Y", "Z", "W"];

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

        for (var componentCount = 2; componentCount <= Components.Length; componentCount++)
        {
            builder.Separate();
            builder.AppendLine($"public struct Vector{componentCount}Int : IEquatable<Vector{componentCount}Int>, IFormattable");
            using (builder.EnterScope())
            {
                for (var i = 0; i < componentCount; i++)
                {
                    builder.AppendLine($"/// <inheritdoc cref=\"Vector{componentCount}.{Components[i]}\"/>");
                    builder.AppendLine($"public int {Components[i]};");
                    builder.AppendLine();
                }

                builder.AppendLine($"/// <inheritdoc cref=\"Vector{componentCount}.Zero\"/>");
                builder.AppendLine($"public static Vector{componentCount}Int Zero => default;");
                builder.Separate();
                builder.AppendLine($"/// <inheritdoc cref=\"Vector{componentCount}.One\"/>");
                builder.AppendLine($"public static Vector{componentCount}Int One => new(1);");

                for (var i = 0; i < componentCount; i++)
                {
                    var currentComponent = i;
                    var parameters = string.Join(", ", Enumerable.Range(0, componentCount).Select(index => index == currentComponent ? "1" : "0"));
                    builder.Separate();
                    builder.AppendLine($"/// <inheritdoc cref=\"Vector{componentCount}.Unit{Components[i]}\"/>");
                    builder.AppendLine($"public static Vector{componentCount}Int Unit{Components[i]} => new({parameters});");
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
                                builder.AppendLine($"case {i}: return {Components[i]};");
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
                                builder.AppendLine($"case {i}: {Components[i]} = value; break;");
                            }
                            builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                        }
                    }
                }

                builder.Separate();
                builder.AppendLine($"public Vector{componentCount}Int(int value) : this({string.Join(", ", Enumerable.Range(0, componentCount).Select(_ => "value"))}) {{}}");

                builder.Separate();
                builder.AppendLine($"public Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"int {Components[index].ToLower()}"))})");
                using (builder.EnterScope())
                {
                    for (var i = 0; i < componentCount; i++)
                    {
                        builder.AppendLine($"{Components[i]} = {Components[i].ToLower()};");
                    }
                }

                builder.Separate();
                builder.AppendLine($"public static explicit operator Vector{componentCount}Int(Vector{componentCount} value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"(int)value.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static implicit operator Vector{componentCount}(Vector{componentCount}Int value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator *(Vector{componentCount}Int value, int scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{Components[index]} * scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount} operator *(Vector{componentCount}Int value, float scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{Components[index]} * scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator /(Vector{componentCount}Int value, int scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{Components[index]} / scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount} operator /(Vector{componentCount}Int value, float scalar)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"value.{Components[index]} / scalar"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator +(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} + right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator -(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} - right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator *(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} * right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator /(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} / right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator <<(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} << right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator >>(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} >> right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator >>>(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} >>> right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator &(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} & right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator |(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} | right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator ^(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return new Vector{componentCount}Int({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"left.{Components[index]} ^ right.{Components[index]}"))});");
                }

                builder.Separate();
                builder.AppendLine($"public static Vector{componentCount}Int operator -(Vector{componentCount}Int value)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return Zero - value;");
                }

                builder.Separate();
                builder.AppendLine($"public static bool operator ==(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return left.Equals(right);");
                }

                builder.Separate();
                builder.AppendLine($"public static bool operator !=(Vector{componentCount}Int left, Vector{componentCount}Int right)");
                using (builder.EnterScope())
                {
                    builder.AppendLine("return !left.Equals(right);");
                }

                builder.Separate();
                builder.AppendLine($"public bool Equals(Vector{componentCount}Int other)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return {string.Join(" && ", Enumerable.Range(0, componentCount).Select(index => $"{Components[index]} == other.{Components[index]}"))};");
                }

                builder.Separate();
                builder.AppendLine("public override bool Equals(object? obj)");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return obj is Vector{componentCount}Int other && Equals(other);");
                }

                builder.Separate();
                builder.AppendLine("public override int GetHashCode()");
                using (builder.EnterScope())
                {
                    builder.AppendLine($"return HashCode.Combine({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"{Components[index]}"))});");
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
                    builder.AppendLine($"return ((Vector{componentCount})this).ToString(format, formatProvider);");
                }
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "VectorXInt.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

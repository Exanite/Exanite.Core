using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class VectorIntGenerator : VectorGenerator
{
    private static readonly string[] AllComponents = ["X", "Y", "Z", "W"];

    public void Run()
    {
        for (var componentCount = 2; componentCount <= AllComponents.Length; componentCount++)
        {
            var components = AllComponents.Take(componentCount).ToArray();
            var vectorIntType = $"Vector{componentCount}Int";
            var vectorFloatType = $"Vector{componentCount}";

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
            using (builder.EnterScope($"public partial struct {vectorIntType} : IEquatable<{vectorIntType}>, IFormattable"))
            {
                foreach (var component in components)
                {
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.{component}\"/>");
                    builder.AppendLine($"public int {component};");
                    builder.AppendLine();
                }

                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Zero\"/>");
                builder.AppendLine($"public static {vectorIntType} Zero => default;");
                builder.AppendLine();
                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.One\"/>");
                builder.AppendLine($"public static {vectorIntType} One => new(1);");

                for (var i = 0; i < componentCount; i++)
                {
                    var currentComponent = i;
                    var parameters = string.Join(", ", Enumerable.Range(0, componentCount).Select(index => index == currentComponent ? "1" : "0"));

                    builder.AppendSeparation();
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Unit{components[i]}\"/>");
                    builder.AppendLine($"public static {vectorIntType} Unit{components[i]} => new({parameters});");
                }

                builder.AppendSeparation();
                using (builder.EnterScope("public int this[int index]"))
                {
                    using (builder.EnterScope("readonly get"))
                    {
                        using (builder.EnterScope("switch (index)"))
                        {
                            for (var i = 0; i < componentCount; i++)
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
                            for (var i = 0; i < componentCount; i++)
                            {
                                builder.AppendLine($"case {i}: {components[i]} = value; break;");
                            }
                            builder.AppendLine("default: throw new IndexOutOfRangeException(nameof(index));");
                        }
                    }
                }

                builder.AppendSeparation();
                builder.AppendLine($"public {vectorIntType}(int value) : this({string.Join(", ", components.Select(_ => "value"))}) {{}}");

                builder.AppendSeparation();
                using (builder.EnterScope($"public {vectorIntType}({string.Join(", ", components.Select(c => $"int {c.ToLower()}"))})"))
                {
                    foreach (var component in components)
                    {
                        builder.AppendLine($"{component} = {component.ToLower()};");
                    }
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public static explicit operator {vectorIntType}({vectorFloatType} value)"))
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", components.Select(c => $"(int)value.{c}"))});");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public static implicit operator {vectorFloatType}({vectorIntType} value)"))
                {
                    builder.AppendLine($"return new {vectorFloatType}({string.Join(", ", components.Select(c => $"value.{c}"))});");
                }

                AppendScalarOperation(builder, components, vectorIntType, "int", vectorIntType, "*");
                AppendScalarOperation(builder, components, vectorIntType, "float", vectorFloatType, "*");

                AppendScalarOperation(builder, components, vectorIntType, "int", vectorIntType, "/");
                AppendScalarOperation(builder, components, vectorIntType, "float", vectorFloatType, "/");

                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "+");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "-");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "*");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "/");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "%");

                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "<<");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, ">>");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, ">>>");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "&");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "|");
                AppendVectorOperation(builder, components, vectorIntType, vectorIntType, vectorIntType, "^");

                builder.AppendSeparation();
                using (builder.EnterScope($"public static {vectorIntType} operator -({vectorIntType} value)"))
                {
                    builder.AppendLine("return Zero - value;");
                }

                AppendEqualityOperations(builder, vectorIntType, components);
                AppendFormattingOperations(builder, components);
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{vectorIntType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

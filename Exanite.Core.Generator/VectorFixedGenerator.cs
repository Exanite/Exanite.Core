using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class VectorFixedGenerator : VectorGenerator
{
    private static readonly string[] AllComponents = ["X", "Y", "Z", "W"];

    public void Run()
    {
        for (var componentCount = 2; componentCount <= AllComponents.Length; componentCount++)
        {
            var components = AllComponents.Take(componentCount).ToArray();
            var vectorFixedType = $"Vector{componentCount}Fixed";
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
            using (builder.EnterScope($"public partial struct {vectorFixedType} : IEquatable<{vectorFixedType}>, IFormattable"))
            {
                foreach (var component in components)
                {
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.{component}\"/>");
                    builder.AppendLine($"public Fixed {component};");
                    builder.AppendLine();
                }

                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Zero\"/>");
                builder.AppendLine($"public static {vectorFixedType} Zero => default;");
                builder.AppendLine();
                builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.One\"/>");
                builder.AppendLine($"public static {vectorFixedType} One => new(1);");

                for (var i = 0; i < componentCount; i++)
                {
                    var currentComponent = i;
                    var parameters = string.Join(", ", Enumerable.Range(0, componentCount).Select(index => index == currentComponent ? "1" : "0"));

                    builder.AppendSeparation();
                    builder.AppendLine($"/// <inheritdoc cref=\"{vectorFloatType}.Unit{components[i]}\"/>");
                    builder.AppendLine($"public static {vectorFixedType} Unit{components[i]} => new({parameters});");
                }

                builder.AppendSeparation();
                using (builder.EnterScope("public Fixed this[int index]"))
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
                builder.AppendLine($"public {vectorFixedType}(Fixed value) : this({string.Join(", ", components.Select(_ => "value"))}) {{}}");

                builder.AppendSeparation();
                using (builder.EnterScope($"public {vectorFixedType}({string.Join(", ", components.Select(c => $"Fixed {c.ToLower()}"))})"))
                {
                    foreach (var component in components)
                    {
                        builder.AppendLine($"{component} = {component.ToLower()};");
                    }
                }

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Safe - No precision loss possible");
                using (builder.EnterScope($"public static implicit operator {vectorFixedType}({vectorIntType} value)"))
                {
                    builder.AppendLine($"return new {vectorFixedType}({string.Join(", ", components.Select(c => $"value.{c}"))});");
                }

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Unsafe - Non-deterministic");
                builder.AppendLine("// Consider using FromFraction instead");
                using (builder.EnterScope($"public static explicit operator {vectorFixedType}({vectorFloatType} value)"))
                {
                    builder.AppendLine($"return new {vectorFixedType}({string.Join(", ", components.Select(c => $"(Fixed)value.{c}"))});");
                }

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Loss of fraction");
                using (builder.EnterScope($"public static explicit operator {vectorIntType}({vectorFixedType} value)"))
                {
                    builder.AppendLine($"return new {vectorIntType}({string.Join(", ", components.Select(c => $"(int)value.{c}"))});");
                }

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Loss of precision");
                using (builder.EnterScope($"public static explicit operator {vectorFloatType}({vectorFixedType} value)"))
                {
                    builder.AppendLine($"return new {vectorFloatType}({string.Join(", ", components.Select(c => $"(float)value.{c}"))});");
                }

                AppendScalarOperation(builder, components, vectorFixedType, "Fixed", vectorFixedType, "*");
                AppendScalarOperation(builder, components, vectorFixedType, "Fixed", vectorFixedType, "/");

                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "+");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "-");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "*");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "/");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "%");

                builder.AppendSeparation();
                using (builder.EnterScope($"public static {vectorFixedType} operator -({vectorFixedType} value)"))
                {
                    builder.AppendLine("return Zero - value;");
                }

                AppendEqualityOperations(builder, vectorFixedType, components);
                AppendFormattingOperations(builder, components);
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{vectorFixedType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

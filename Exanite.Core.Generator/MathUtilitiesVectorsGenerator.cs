using System.Collections.Generic;
using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class MathUtilitiesVectorsGenerator
{
    public void Run()
    {
        var components = GeneratorConstants.VectorComponents;
        var vectors = new List<VectorType>()
        {
            new("", "float"),

            new("Int", "int")
            {
                IsIntegerBacked = true,
            },

            new("Fixed", "Fixed")
            {
                IsFixedBacked = true,
            },
        };

        foreach (var vector in vectors)
        {
            var suffix = vector.Suffix;
            var backingType = vector.BackingType;
            var isIntegerBacked = vector.IsIntegerBacked;
            var isFixedBacked = vector.IsFixedBacked;

            for (var componentCount = 2; componentCount <= components.Length; componentCount++)
            {
                var vectorType = $"Vector{componentCount}{suffix}";

                var builder = new IndentedStringBuilder();
                builder.AppendGeneratedCodeHeader();

                builder.AppendLine("using System;");
                builder.AppendLine("using System.Numerics;");
                builder.AppendLine("using Exanite.Core.Numerics;");
                builder.AppendLine();
                builder.AppendLine("namespace Exanite.Core.Utilities;");
                builder.AppendLine();
                builder.AppendLine("public static partial class M");

                using (builder.EnterScope())
                {
                    if (!isIntegerBacked)
                    {
                        builder.AppendLine("/// <summary>");
                        builder.AppendLine("/// Interpolates from one vector to another by <see cref=\"t\"/>.");
                        builder.AppendLine("/// <see cref=\"t\"/> will be clamped in the range [0, 1]");
                        builder.AppendLine("/// </summary>");
                        using (builder.EnterScope($"public static {vectorType} Lerp({vectorType} from, {vectorType} to, {backingType} t)"))
                        {
                            builder.AppendLine("t = Clamp01(t);");
                            builder.AppendLine("return from + (to - from) * t;");
                        }
                        builder.AppendLine();

                        builder.AppendLine("/// <summary>");
                        builder.AppendLine("/// Interpolates from one vector to another by <see cref=\"t\"/>.");
                        builder.AppendLine("/// </summary>");
                        using (builder.EnterScope($"public static {vectorType} LerpUnclamped({vectorType} from, {vectorType} to, {backingType} t)"))
                        {
                            builder.AppendLine("return from + (to - from) * t;");
                        }
                        builder.AppendLine();

                        if (!isFixedBacked)
                        {
                            builder.AppendLine("/// <inheritdoc cref=\"SmoothDamp{T}\"/>/>");
                            using (builder.EnterScope($"public static {vectorType} SmoothDamp({vectorType} current, {vectorType} target, {backingType} smoothTime, {backingType} deltaTime, ref {vectorType} currentVelocity, {backingType} maxSpeed = float.PositiveInfinity)"))
                            {
                                builder.AppendLine($"var result = new {vectorType}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"SmoothDamp(current.{components[index]}, target.{components[index]}, smoothTime, deltaTime, ref currentVelocity.{components[index]}, maxSpeed)"))});");
                                builder.AppendLine("currentVelocity = ClampMagnitude(currentVelocity, maxSpeed);");
                                builder.AppendLine();
                                builder.AppendLine("return result;");
                            }
                            builder.AppendLine();

                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// Clamps the length of the provided vector to between [0, maxLength].");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static {vectorType} ClampMagnitude({vectorType} value, {backingType} maxLength)"))
                            {
                                builder.AppendLine("return ClampMagnitude(value, 0, maxLength);");
                            }
                            builder.AppendLine();

                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// Clamps the length of the provided vector to between [minLength, maxLength].");
                            builder.AppendLine("/// If a zero vector is provided, then the result is a zero vector.");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static {vectorType} ClampMagnitude({vectorType} value, {backingType} minLength, {backingType} maxLength)"))
                            {
                                builder.AppendLine("return value.AsNormalizedOrDefault() * Clamp(value.Length(), minLength, maxLength);");
                            }
                            builder.AppendLine();

                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// Returns the normalized version of the provided vector, or returns zero if the provided vector is zero.");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static {vectorType} AsNormalizedOrDefault(this {vectorType} value)"))
                            {
                                builder.AppendLine($"return value.AsNormalizedOrDefault({vectorType}.Zero);");
                            }
                            builder.AppendLine();

                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// Returns the normalized version of the provided vector, or returns the specified default value if the provided vector is zero.");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static {vectorType} AsNormalizedOrDefault(this {vectorType} value, {vectorType} defaultValue)"))
                            {
                                builder.AppendLine($"return value == {vectorType}.Zero ? defaultValue : {vectorType}.Normalize(value);");
                            }
                            builder.AppendLine();

                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// Checks if two vectors are approximately the same value based on the provided <see cref=\"tolerance\"/>.");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static bool ApproximatelyEquals({vectorType} a, {vectorType} b, {backingType} tolerance = 0.000001f)"))
                            {
                                builder.AppendLine($"return {string.Join(" && ", Enumerable.Range(0, componentCount).Select(index => $"ApproximatelyEquals(a.{components[index]}, b.{components[index]}, tolerance)"))};");
                            }
                        }
                    }

                    builder.AppendLine("/// <summary>");
                    builder.AppendLine("/// Component-wise clamps the provided vector to the bounds given by <see cref=\"min\"/> and <see cref=\"max\"/>.");
                    builder.AppendLine("/// </summary>");
                    using (builder.EnterScope($"public static void Clamp(ref this {vectorType} vector, {vectorType} min, {vectorType} max)"))
                    {
                        for (var i = 0; i < componentCount; i++)
                        {
                            builder.AppendLine($"vector.{components[i]} = Clamp(vector.{components[i]}, min.{components[i]}, max.{components[i]});");
                        }
                    }
                }

                var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Utilities" / $"MathUtility.{vectorType}.g.cs";
                outputPath.WriteAllText(builder.ToString());
            }
        }
    }

    private readonly record struct VectorType(string Suffix, string BackingType)
    {
        public bool IsIntegerBacked { get; init; } = false;
        public bool IsFixedBacked { get; init; } // TODO: Temporary config for excluding certain methods from fixed type vectors
    }
}

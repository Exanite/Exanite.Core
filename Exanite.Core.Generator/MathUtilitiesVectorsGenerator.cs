using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

// TODO: This needs cleanup and also added support for VectorXFixed types
public class MathUtilitiesVectorsGenerator
{
    public void Run()
    {
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
            var components = GeneratorConstants.VectorComponents;
            for (var componentCount = 2; componentCount <= components.Length; componentCount++)
            {
                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Interpolates from one vector to another by <see cref=\"t\"/>.");
                builder.AppendLine("/// <see cref=\"t\"/> will be clamped in the range [0, 1]");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} Lerp(Vector{componentCount} from, Vector{componentCount} to, float t)"))
                {
                    builder.AppendLine("t = Clamp01(t);");
                    builder.AppendLine("return from + (to - from) * t;");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Interpolates from one vector to another by <see cref=\"t\"/>.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} LerpUnclamped(Vector{componentCount} from, Vector{componentCount} to, float t)"))
                {
                    builder.AppendLine("return from + (to - from) * t;");
                }
                builder.AppendLine();

                builder.AppendLine("/// <inheritdoc cref=\"SmoothDamp{T}\"/>/>");
                using (builder.EnterScope($"public static Vector{componentCount} SmoothDamp(Vector{componentCount} current, Vector{componentCount} target, float smoothTime, float deltaTime, ref Vector{componentCount} currentVelocity, float maxSpeed = float.PositiveInfinity)"))
                {
                    builder.AppendLine($"var result = new Vector{componentCount}({string.Join(", ", Enumerable.Range(0, componentCount).Select(index => $"SmoothDamp(current.{components[index]}, target.{components[index]}, smoothTime, deltaTime, ref currentVelocity.{components[index]}, maxSpeed)"))});");
                    builder.AppendLine("currentVelocity = ClampMagnitude(currentVelocity, maxSpeed);");
                    builder.AppendLine();
                    builder.AppendLine("return result;");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Clamps the length of the provided vector to between [0, maxLength].");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} ClampMagnitude(Vector{componentCount} value, float maxLength)"))
                {
                    builder.AppendLine("return ClampMagnitude(value, 0, maxLength);");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Clamps the length of the provided vector to between [minLength, maxLength].");
                builder.AppendLine("/// If a zero vector is provided, then the result is a zero vector.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} ClampMagnitude(Vector{componentCount} value, float minLength, float maxLength)"))
                {
                    builder.AppendLine("return value.AsNormalizedOrDefault() * Clamp(value.Length(), minLength, maxLength);");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Component-wise clamps the provided vector to the bounds given by <see cref=\"min\"/> and <see cref=\"max\"/>.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static void Clamp(ref this Vector{componentCount} vector, Vector{componentCount} min, Vector{componentCount} max)"))
                {
                    for (var i = 0; i < componentCount; i++)
                    {
                        builder.AppendLine($"vector.X = Clamp(vector.{components[i]}, min.{components[i]}, max.{components[i]});");
                    }
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Returns the normalized version of the provided vector, or returns zero if the provided vector is zero.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} AsNormalizedOrDefault(this Vector{componentCount} value)"))
                {
                    builder.AppendLine($"return value.AsNormalizedOrDefault(Vector{componentCount}.Zero);");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Returns the normalized version of the provided vector, or returns the specified default value if the provided vector is zero.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static Vector{componentCount} AsNormalizedOrDefault(this Vector{componentCount} value, Vector{componentCount} defaultValue)"))
                {
                    builder.AppendLine($"return value == Vector{componentCount}.Zero ? defaultValue : Vector{componentCount}.Normalize(value);");
                }
                builder.AppendLine();

                builder.AppendLine("/// <summary>");
                builder.AppendLine("/// Checks if two vectors are approximately the same value based on the provided <see cref=\"tolerance\"/>.");
                builder.AppendLine("/// </summary>");
                using (builder.EnterScope($"public static bool ApproximatelyEquals(Vector{componentCount} a, Vector{componentCount} b, float tolerance = 0.000001f)"))
                {
                    builder.AppendLine($"return {string.Join(" && ", Enumerable.Range(0, componentCount).Select(index => $"ApproximatelyEquals(a.{components[index]}, b.{components[index]}, tolerance)"))};");
                }
            }

            for (var isVectorIntI = 0; isVectorIntI < 2; isVectorIntI++)
            {
                var suffix = isVectorIntI == 1 ? "Int" : "";

                for (var fromCount = 2; fromCount <= components.Length; fromCount++)
                {
                    for (var toCount = 2; toCount <= components.Length; toCount++)
                    {
                        if (fromCount == toCount)
                        {
                            continue;
                        }

                        var name = "";
                        var usedComponentCount = Math.Min(fromCount, toCount);
                        for (var usedComponentI = 0; usedComponentI < usedComponentCount; usedComponentI++)
                        {
                            var component = components[usedComponentI];
                            name += usedComponentI == 0 ? component : component.ToLower();
                        }

                        var isDroppingComponents = toCount < fromCount;
                        if (isDroppingComponents)
                        {
                            builder.AppendLine();
                            builder.AppendLine("/// <summary>");
                            builder.AppendLine($"/// Converts a Vector{fromCount}{suffix} to a Vector{toCount}{suffix} by dropping components.");
                            builder.AppendLine("/// </summary>");
                            using (builder.EnterScope($"public static Vector{toCount}{suffix} {name}(this Vector{fromCount}{suffix} value)"))
                            {
                                builder.AppendLine($"return new Vector{toCount}{suffix}({string.Join(", ", Enumerable.Range(0, toCount).Select(index => $"value.{components[index]}"))});");
                            }
                        }
                        else
                        {
                            var addedComponents = new List<string>() { "" };

                            for (var addedComponentI = fromCount; addedComponentI < toCount; addedComponentI++)
                            {
                                var newAddedComponents = new List<string>();
                                foreach (var addedComponent in addedComponents)
                                {
                                    for (var componentToAdd = 0; componentToAdd < 2; componentToAdd++)
                                    {
                                        newAddedComponents.Add(addedComponent + componentToAdd);
                                    }
                                }
                                addedComponents = newAddedComponents;
                            }

                            foreach (var addedComponent in addedComponents)
                            {
                                builder.AppendLine();
                                builder.AppendLine("/// <summary>");
                                builder.AppendLine($"/// Converts a Vector{fromCount}{suffix} to a Vector{toCount}{suffix} by adding components.");
                                builder.AppendLine("/// </summary>");
                                using (builder.EnterScope($"public static Vector{toCount}{suffix} {name}{addedComponent}(this Vector{fromCount}{suffix} value)"))
                                {
                                    builder.AppendLine($"return new Vector{toCount}{suffix}({string.Join(", ", Enumerable.Range(0, fromCount).Select(index => $"value.{components[index]}"))}, {string.Join(", ", addedComponent.Select(c => c))});");
                                }
                            }
                        }
                    }
                }
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Utilities" / "MathUtility.Vectors.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

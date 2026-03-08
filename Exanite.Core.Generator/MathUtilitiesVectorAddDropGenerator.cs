using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class MathUtilitiesVectorAddDropGenerator
{
    public void Run()
    {
        var components = GeneratorConstants.VectorComponents;
        var vectorTypeSuffixes = new List<string>()
        {
            "",
            "Int",
            "Fixed",
        };

        foreach (var suffix in vectorTypeSuffixes)
        {
            var builder = new IndentedStringBuilder();
            builder.AppendGeneratedCodeHeader();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Numerics;");
            builder.AppendLine("using Exanite.Core.Numerics;");
            builder.AppendLine();
            builder.AppendLine("namespace Exanite.Core.Utilities;");
            builder.AppendLine();
            using (builder.EnterScope("public static partial class M"))
            {
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
                            builder.AppendSeparation();
                            builder.AppendLine("/// <summary>");
                            builder.AppendLine($"/// Converts a <see cref=\"Vector{fromCount}{suffix}\"/> to a <see cref=\"Vector{toCount}{suffix}\"/> by dropping components.");
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
                                builder.AppendSeparation();
                                builder.AppendLine("/// <summary>");
                                builder.AppendLine($"/// Converts a <see cref=\"Vector{fromCount}{suffix}\"/> to a <see cref=\"Vector{toCount}{suffix}\"/> by adding components.");
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

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Utilities" / $"MathUtility.Vector{suffix}.AddDrop.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

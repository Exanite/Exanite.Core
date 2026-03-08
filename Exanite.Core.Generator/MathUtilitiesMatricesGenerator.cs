using System.Collections.Generic;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator;

public class MathUtilitiesMatricesGenerator
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

        builder.AppendSeparation();
        using (builder.EnterScope("public static partial class M"))
        {
            using (builder.EnterScope("extension(Matrix4x4)"))
            {
                var components = new List<string>() { "X", "Y", "Z", "W" };
                for (var targetComponentI = 0; targetComponentI < components.Count; targetComponentI++)
                {
                    for (var basedOnComponentI = 0; basedOnComponentI < components.Count; basedOnComponentI++)
                    {
                        if (targetComponentI == basedOnComponentI)
                        {
                            continue;
                        }

                        builder.AppendSeparation();
                        builder.AppendLine("/// <summary>");
                        builder.AppendLine($"/// Creates a matrix for skewing positions on the {components[targetComponentI]}-axis based on the {components[basedOnComponentI]}-axis.");
                        builder.AppendLine("/// </summary>");
                        using (builder.EnterScope($"public static Matrix4x4 CreateSkew{components[targetComponentI]}With{components[basedOnComponentI]}(float amount)"))
                        {
                            builder.AppendLine("var k = amount;");
                            using (builder.Indent("return new Matrix4x4("))
                            {
                                for (var x = 0; x < 4; x++)
                                {
                                    var row = new List<string>();
                                    for (var y = 0; y < 4; y++)
                                    {
                                        if (x == y)
                                        {
                                            row.Add("1");
                                            continue;
                                        }

                                        if (x == targetComponentI && y == basedOnComponentI)
                                        {
                                            row.Add("k");
                                            continue;
                                        }

                                        row.Add("0");
                                    }

                                    builder.AppendLine($"{string.Join(", ", row)}{(x != 3 ? "," : "")}");
                                }
                            }
                            builder.AppendLine(");");
                        }
                    }
                }
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Utilities" / "MathUtility.Matrices.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

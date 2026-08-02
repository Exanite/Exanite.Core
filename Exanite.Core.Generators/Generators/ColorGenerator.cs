using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;
using static Exanite.Core.Generators.GeneratorConstants;

namespace Exanite.Core.Generators.Generators;

public class ColorGenerator
{
    public void Run()
    {
        var builder = new IndentedStringBuilder();
        builder.AppendGeneratedCodeHeader();

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Numerics;");
        builder.AppendLine("using Exanite.Core.Utilities;");
        builder.AppendLine();
        builder.AppendLine("namespace Exanite.Core.Numerics;");

        builder.AppendSeparation();
        using (builder.EnterScope("public partial record struct Color"))
        {
            foreach (var currentType in Colors.Types)
            {
                builder.AppendSeparation();
                builder.AppendLine($"// {currentType.Type}");

                builder.AppendSeparation();
                builder.AppendLine($"public readonly Color {currentType.Name} => As(ColorType.{currentType.Name});");

                builder.AppendSeparation();
                using (builder.EnterScope($"public static Color From{currentType.Name}(Vector3 value)"))
                {
                    builder.AppendLine($"return new Color(value.Xyz1(), ColorType.{currentType.Name});");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public static Color From{currentType.Name}(Vector4 value)"))
                {
                    builder.AppendLine($"return new Color(value, ColorType.{currentType.Name});");
                }

                builder.AppendSeparation();
                using (builder.EnterScope($"public static Color From{currentType.Name}({string.Join(", ", currentType.Components.Select(x => $"float {x.ToLower()}"))} = 1)"))
                {
                    builder.AppendLine($"return new Color(new Vector4({string.Join(", ", currentType.Components.Select(x => x.ToLower()))}), ColorType.{currentType.Name});");
                }
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Color.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

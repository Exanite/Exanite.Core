using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generators.Generators;

public class RectIntGenerator : RectGenerator
{
    public void Run()
    {
        for (var componentCount = 2; componentCount <= 3; componentCount++)
        {
            var components = GeneratorConstants.VectorComponents.Take(componentCount).ToArray();

            var rectType = $"Rect{componentCount}Int";
            var vectorType = $"Vector{componentCount}Int";

            var builder = new IndentedStringBuilder();
            builder.AppendGeneratedCodeHeader();

            builder.AppendLine("using System.Numerics;");
            builder.AppendLine();
            builder.AppendLine("namespace Exanite.Core.Numerics;");

            builder.AppendSeparation();
            using (builder.EnterScope($"public partial record struct {rectType}"))
            {
                AppendConstants(builder, rectType, vectorType);
                AppendFields(builder, vectorType);

                AppendRectCastOperation(builder, "implicit", rectType, $"Rect{componentCount}", $"Vector{componentCount}");
                AppendRectCastOperation(builder, "explicit", $"Rect{componentCount}", rectType, vectorType);

                AppendCreateOperations(builder, rectType, vectorType);
                AppendScaleOperation(builder, rectType, vectorType);
                AppendContainsOperation(builder, vectorType, components);
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{rectType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}
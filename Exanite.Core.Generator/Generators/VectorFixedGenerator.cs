using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator.Generators;

public class VectorFixedGenerator : VectorGenerator
{
    public void Run()
    {
        for (var componentCount = 2; componentCount <= GeneratorConstants.VectorComponents.Length; componentCount++)
        {
            var components = GeneratorConstants.VectorComponents.Take(componentCount).ToArray();

            var fixedType = "Fixed";
            var intType = "int";
            var floatType = "float";

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
                AppendComponentFields(builder, fixedType, components);

                AppendIdentityVectorConstants(builder, vectorFixedType, components);
                AppendBasisVectorConstants(builder, vectorFixedType, components);

                AppendIndexer(builder, fixedType, components);

                AppendConstructors(builder, vectorFixedType, fixedType, components);

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Safe - No precision loss possible");
                AppendVectorCastOperation(builder, "implicit", vectorIntType, vectorFixedType, fixedType, components, true);

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Unsafe - Non-deterministic");
                builder.AppendLine("// Consider using Fixed.FromParts or Fixed.FromFraction instead");
                AppendVectorCastOperation(builder, "explicit", vectorFloatType, vectorFixedType, fixedType, components, true);

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Loss of fraction");
                AppendVectorCastOperation(builder, "explicit", vectorFixedType, vectorIntType, intType, components, true);

                builder.AppendSeparation();
                builder.AppendLine("// Conversion: Loss of precision / determinism");
                AppendVectorCastOperation(builder, "explicit", vectorFixedType, vectorFloatType, floatType, components, true);

                AppendScalarOperation(builder, components, vectorFixedType, fixedType, vectorFixedType, "*");
                AppendScalarOperation(builder, components, vectorFixedType, fixedType, vectorFixedType, "/");

                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "+");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "-");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "*");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "/");
                AppendVectorOperation(builder, components, vectorFixedType, vectorFixedType, vectorFixedType, "%");

                AppendNegateOperation(builder, vectorFixedType);

                AppendEqualityOperations(builder, vectorFixedType, components);
                AppendFormattingOperations(builder, components);
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{vectorFixedType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

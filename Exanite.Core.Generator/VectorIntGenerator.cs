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

            var intType = "int";
            var floatType = "float";

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
                AppendComponentFields(builder, intType, components);

                AppendIdentityVectorConstants(builder, vectorIntType, components);
                AppendBasisVectorConstants(builder, vectorIntType, components);

                AppendIndexer(builder, intType, components);

                AppendConstructors(builder, vectorIntType, intType, components);

                AppendVectorCastOperation(builder, "explicit", vectorFloatType, vectorIntType, intType, components);
                AppendVectorCastOperation(builder, "implicit", vectorIntType, vectorFloatType, floatType, components);

                AppendScalarOperation(builder, components, vectorIntType, intType, vectorIntType, "*");
                AppendScalarOperation(builder, components, vectorIntType, floatType, vectorFloatType, "*");

                AppendScalarOperation(builder, components, vectorIntType, intType, vectorIntType, "/");
                AppendScalarOperation(builder, components, vectorIntType, floatType, vectorFloatType, "/");

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

                AppendNegateOperation(builder, vectorIntType);

                AppendEqualityOperations(builder, vectorIntType, components);
                AppendFormattingOperations(builder, components);
            }

            var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / $"{vectorIntType}.g.cs";
            outputPath.WriteAllText(builder.ToString());
        }
    }
}

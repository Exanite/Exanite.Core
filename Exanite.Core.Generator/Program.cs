using Exanite.Core.Io;
using Exanite.Core.Utilities;

namespace Exanite.Core.Generator;

public class Program
{
    public static void Main()
    {
        var csprojFile = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Exanite.Core.csproj";
        GuardUtility.IsTrue(csprojFile.Exists, "Working directory is incorrect. Please set it to be the root of the Exanite.Core repo");

        new VectorIntGenerator().Run();
        new VectorFixedGenerator().Run();

        new MathUtilitiesMatricesGenerator().Run();
        new MathUtilitiesVectorsGenerator().Run();
        new MathUtilitiesVectorAddDropGenerator().Run();
    }
}

using System.Collections.Generic;
using Exanite.CodeGen;
using Exanite.Core.Io;

namespace Exanite.Core.Generator.Generators;

public class FixedLookupsGenerator
{
    public void Run()
    {
        var builder = new IndentedStringBuilder();
        builder.AppendGeneratedCodeHeader();

        builder.AppendLine("using System.Collections.Immutable;");
        builder.AppendLine();
        builder.AppendLine("namespace Exanite.Core.Numerics;");

        builder.AppendSeparation();
        using (builder.EnterScope("public partial struct Fixed"))
        {
            // Digit lookup
            {
                var digits = new List<int>();
                var limit = 10;
                try
                {
                    checked
                    {
                        while (true)
                        {
                            digits.Add(limit - 1);
                            limit *= 10;
                        }
                    }
                }
                catch
                {
                    digits.Add(int.MaxValue);
                }

                using (builder.Indent("public static readonly ImmutableArray<int> DigitsLut = ["))
                {
                    foreach (var digit in digits)
                    {
                        builder.AppendLine($"{digit.ToString()},");
                    }
                }
                builder.AppendLine("];");
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Fixed.Lookups.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

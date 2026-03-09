using System.Collections.Generic;
using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Generator.Generators;

public class FixedLookupsGenerator
{
    private readonly bool generateLookups;

    public FixedLookupsGenerator(bool generateLookups)
    {
        this.generateLookups = generateLookups;
    }

    public void Run()
    {
        if (!generateLookups)
        {
            return;
        }

        var builder = new IndentedStringBuilder();
        builder.AppendGeneratedCodeHeader();

        builder.AppendLine("using System.Collections.Immutable;");
        builder.AppendLine();
        builder.AppendLine("namespace Exanite.Core.Numerics;");

        builder.AppendSeparation();
        using (builder.EnterScope("public partial struct Fixed"))
        {
            // Digit lookup
            // This stores max values corresponding to each digit count
            {
                var entries = new List<int>();
                var limit = 10;
                try
                {
                    checked
                    {
                        while (true)
                        {
                            entries.Add(limit - 1);
                            limit *= 10;
                        }
                    }
                }
                catch
                {
                    entries.Add(int.MaxValue);
                }

                builder.AppendSeparation();
                using (builder.Indent("public static readonly ImmutableArray<int> DigitsLut = ["))
                {
                    foreach (var entry in entries)
                    {
                        builder.AppendLine($"{entry.ToString()},");
                    }
                }
                builder.AppendLine("];");
            }

            // TODO: This is my initial guess at what we need to do
            // Sqrt lookup
            // This stores initial guesses for y for the Inverse Newton-Raphsom method
            // The guesses have the value 1 / sqrt(x_normalized) and are indexed using the upper n bits of x_normalized
            // x_normalized is in the range [0.25, 2)
            {
                var lookupBits = 4; // TODO: Increase
                var lookupEntryCount = 1 << lookupBits;

                var doubleEntries = new List<double>();
                for (var i = 0; i < lookupEntryCount; i++)
                {
                    // TODO: This is probably wrong
                    var t = (double)i / (lookupEntryCount + 1);
                    var xNormalized = M.Lerp(0.5, 2, t);
                    var inverseSqrt = 1 / double.Sqrt(xNormalized);

                    doubleEntries.Add(inverseSqrt);
                }

                var fixedEntries = doubleEntries.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = fixedEntries.Max(x => x.Length);
                var valuesPerLine = 8;

                builder.AppendSeparation();
                builder.AppendLine($"public const int SqrtLutBits = {lookupBits};");
                using (builder.Indent("public static readonly ImmutableArray<int> SqrtLut = ["))
                {
                    for (var i = 0; i < fixedEntries.Count; i++)
                    {
                        var entry = fixedEntries[i];
                        builder.Append($"{entry.PadLeft(entryMaxLength)}");
                        if (i % valuesPerLine == valuesPerLine - 1)
                        {
                            builder.AppendLine(",");
                        }
                        else
                        {
                            builder.Append(", ");
                        }
                    }
                }
                builder.AppendLine("];");
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Fixed.Lookup.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

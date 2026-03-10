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

        GenerateFixedLookups();
        GenerateFixed128Lookups();
    }

    private void GenerateFixedLookups()
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

            // Sine lookup
            // This stores sine values for the range [0pi, pi/2)
            //
            // The size of the LUT affects precision
            {
                var lookupBits = 7;
                var lookupEntryCount = 1 << lookupBits;

                var sineValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => double.Sin(i * (double.Pi / 2) / lookupEntryCount))
                    .ToList();

                var tableEntries = sineValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"public const int SinLutBits = {lookupBits};");
                using (builder.Indent("public static readonly ImmutableArray<ushort> SinLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }

            // Tangent lookup
            // This stores tangent values for the range [0pi, pi/2)
            {
                var lookupBits = 13;
                var lookupEntryCount = 1 << lookupBits;

                var tangentValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => double.Tan(i * (double.Pi / 2) / lookupEntryCount))
                    .ToList();

                var tableEntries = tangentValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"public const int TanLutBits = {lookupBits};");
                using (builder.Indent("public static readonly ImmutableArray<uint> TanLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }

            // Arctangent lookup
            // This stores tangent values for the range [0, 1)
            {
                var lookupBits = 6;
                var lookupEntryCount = 1 << lookupBits;

                var arctangentValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => double.Atan((double)i / lookupEntryCount))
                    .ToList();

                var tableEntries = arctangentValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"public const int AtanLutBits = {lookupBits};");
                using (builder.Indent("public static readonly ImmutableArray<ushort> AtanLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Fixed.Lookup.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }

    private void GenerateFixed128Lookups()
    {
        var builder = new IndentedStringBuilder();
        builder.AppendGeneratedCodeHeader();

        builder.AppendLine("using System.Collections.Immutable;");
        builder.AppendLine();
        builder.AppendLine("namespace Exanite.Core.Numerics;");

        builder.AppendSeparation();
        using (builder.EnterScope("public partial struct Fixed128"))
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

            // Sqrt lookup
            // This stores initial guesses for y for the Inverse Newton-Raphsom method
            // The guesses have the value 1 / sqrt(x_normalized) and are indexed using the upper n bits of x_normalized
            // x_normalized is in the range [0.5, 2)
            //
            // The size of the LUT affects performance
            // The size does not affect precision
            {
                // Note that we drop the first quarter of the table here
                var lookupBits = 10;
                var lookupEntryCount = 1 << lookupBits;
                var lookupOffset = lookupEntryCount / 4;
                lookupEntryCount -= lookupOffset;

                var xNormalizedValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => M.Lerp(0.5, 2, (double)i / lookupEntryCount))
                    .ToList();

                var inverseRootValues = xNormalizedValues
                    .Select(x => 1 / double.Sqrt(x))
                    .ToList();

                var tableEntries = inverseRootValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"public const int SqrtLutBits = {lookupBits};");
                builder.AppendLine($"public const int SqrtLutOffset = {lookupOffset};");
                builder.AppendLine("public const int SqrtLutShift = 16;");
                using (builder.Indent("public static readonly ImmutableArray<uint> SqrtLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Fixed128.Lookup.g.cs";
        outputPath.WriteAllText(builder.ToString());
    }
}

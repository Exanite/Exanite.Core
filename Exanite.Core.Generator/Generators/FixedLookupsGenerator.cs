using System.Collections.Generic;
using System.Linq;
using Exanite.CodeGen;
using Exanite.Core.Io;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Generator.Generators;

public class FixedLookupsGenerator
{
    private readonly bool generate;

    public FixedLookupsGenerator(bool generate)
    {
        this.generate = generate;
    }

    public void Run()
    {
        if (!generate)
        {
            return;
        }

        GenerateFixedConstants();
        GenerateFixedLookups();
        GenerateFixed128Lookups();
    }

    private void GenerateFixedConstants()
    {
        var builder = new IndentedStringBuilder();
        builder.AppendGeneratedCodeHeader();

        builder.AppendLine("namespace Exanite.Core.Numerics;");

        builder.AppendSeparation();
        using (builder.EnterScope("public partial struct Fixed"))
        {
            builder.AppendBlock($"""
                private const int ERaw = {CalculateRaw(double.E)};
                private const int PiRaw = {CalculateRaw(double.Pi)};
                private const int PiHalfRaw = {CalculateRaw(double.Pi / 2)};
                private const int PiFourthRaw = {CalculateRaw(double.Pi / 4)};
                private const int PiInverseRaw = {CalculateRaw(1 / double.Pi)};
                private const int TauRaw = {CalculateRaw(double.Tau)};

                private const int LogETwoRaw = {CalculateRaw(double.Log(2))};
                private const int Log10TwoRaw = {CalculateRaw(double.Log10(2))};
                """);

            // These round up instead since Taylor series will always underestimate
            builder.AppendSeparation();
            builder.AppendBlock($"""
                // Exp2 Taylor Series Constants
                private const ushort Exp2Term1 = {CalculateRawCeiling(double.Log(2))};
                private const ushort Exp2Term2 = {CalculateRawCeiling(double.Pow(double.Log(2), 2) / 2)};
                private const ushort Exp2Term3 = {CalculateRawCeiling(double.Pow(double.Log(2), 3) / 6)};
                private const ushort Exp2Term4 = {CalculateRawCeiling(double.Pow(double.Log(2), 4) / 24)};
                private const ushort Exp2Term5 = {CalculateRawCeiling(double.Pow(double.Log(2), 5) / 120)};
                """);

            builder.AppendSeparation();
            builder.AppendBlock($"""
                /// <summary>
                /// Q4.60 format.
                /// </summary>
                private const long TauPreciseRaw = {(long)decimal.Round((decimal)double.Tau * (1L << 60))};
                private const int TauPreciseShift = 60;

                /// <summary>
                /// Q4.60 format.
                /// </summary>
                private const long PiPreciseRaw = {(long)decimal.Round((decimal)double.Pi * (1L << 60))};
                private const int PiPreciseShift = 60;
                """);
        }

        var outputPath = AbsolutePath.WorkingDirectory / "Exanite.Core" / "Numerics" / "Fixed.Constants.g.cs";
        outputPath.WriteAllText(builder.ToString());

        return;

        long CalculateRaw(double constant)
        {
            return (long)double.Round(constant * (1L << Fixed.Shift));
        }

        long CalculateRawCeiling(double constant)
        {
            return (long)double.Ceiling(constant * (1L << Fixed.Shift));
        }
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
                using (builder.Indent("private static readonly ImmutableArray<int> DigitsLut = ["))
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
                builder.AppendLine($"private const int SinLutBits = {lookupBits};");
                using (builder.Indent("private static readonly ImmutableArray<ushort> SinLut = ["))
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
                builder.AppendLine($"private const int TanLutBits = {lookupBits};");
                using (builder.Indent("private static readonly ImmutableArray<uint> TanLut = ["))
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
                builder.AppendLine($"private const int AtanLutBits = {lookupBits};");
                using (builder.Indent("private static readonly ImmutableArray<ushort> AtanLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }

            // Log2 lookup
            // This stores log2 values for the range [1, 2)
            {
                var lookupBits = 13;
                var lookupEntryCount = 1 << lookupBits;

                var log2Values = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => double.Log2(M.Lerp(1, 2, (i + 0.5) / lookupEntryCount)))
                    .ToList();

                var tableEntries = log2Values.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"private const int Log2LutBits = {lookupBits};");
                using (builder.Indent("private static readonly ImmutableArray<ushort> Log2Lut = ["))
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
                using (builder.Indent("private static readonly ImmutableArray<int> DigitsLut = ["))
                {
                    foreach (var entry in entries)
                    {
                        builder.AppendLine($"{entry.ToString()},");
                    }
                }
                builder.AppendLine("];");
            }

            // Sqrt lookup
            // This stores initial guesses for y for the Inverse Newton-Raphson method
            // The guesses have the value 1 / sqrt(x_normalized) and are indexed using the upper n bits of x_normalized
            // x_normalized is in the range [0.5, 2)
            //
            // The size of the LUT affects performance
            // The size does not affect precision (unless we don't have enough iterations to convert)
            {
                // Note that we drop the first quarter of the table here
                var lookupBits = 10;
                var lookupEntryCount = 1 << lookupBits;
                var lookupOffset = lookupEntryCount / 4;
                lookupEntryCount -= lookupOffset;

                var xNormalizedValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => M.Lerp(0.5, 2, (double)i / lookupEntryCount))
                    .ToList();

                var inverseSqrtValues = xNormalizedValues
                    .Select(x => 1 / double.Sqrt(x))
                    .ToList();

                var tableEntries = inverseSqrtValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"private const int SqrtLutBits = {lookupBits};");
                builder.AppendLine($"private const int SqrtLutOffset = {lookupOffset};");
                builder.AppendLine("private const int SqrtLutShift = 16;");
                using (builder.Indent("private static readonly ImmutableArray<uint> SqrtLut = ["))
                {
                    foreach (var chunk in tableEntries.Chunk(valuesPerLine))
                    {
                        builder.AppendLine($"{string.Join(", ", chunk.Select(x => x.PadLeft(entryMaxLength)))},");
                    }
                }
                builder.AppendLine("];");
            }

            // Cbrt lookup
            // This stores initial guesses for y for the Direct Newton-Raphson method
            // The guesses have the value cbrt(x_normalized) and are indexed using the upper n bits of x_normalized
            // x_normalized is in the range [0.25, 2)
            //
            // The size of the LUT affects performance
            // The size does not affect precision (unless we don't have enough iterations to convert)
            {
                // Note that we drop the first eighth of the table here
                var lookupBits = 10;
                var lookupEntryCount = 1 << lookupBits;
                var lookupOffset = lookupEntryCount / 8;
                lookupEntryCount -= lookupOffset;

                var xNormalizedValues = Enumerable.Range(0, lookupEntryCount)
                    .Select(i => M.Lerp(0.25, 2, (double)i / lookupEntryCount))
                    .ToList();

                var cbrtValues = xNormalizedValues
                    .Select(x => double.Cbrt(x))
                    .ToList();

                var tableEntries = cbrtValues.Select(x => (long)(x * (1 << Fixed.FractionalBitCount))).Select(x => x.ToString()).ToList();
                var entryMaxLength = tableEntries.Max(x => x.Length);
                var valuesPerLine = 16;

                builder.AppendSeparation();
                builder.AppendLine($"private const int CbrtLutBits = {lookupBits};");
                builder.AppendLine($"private const int CbrtLutOffset = {lookupOffset};");
                builder.AppendLine("private const int CbrtLutShift = 16;");
                using (builder.Indent("private static readonly ImmutableArray<uint> CbrtLut = ["))
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

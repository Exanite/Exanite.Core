using System;
using System.Numerics;
using Exanite.Core.Utilities;
using DrawingColor = System.Drawing.Color;

namespace Exanite.Core.Numerics;

/// <summary>
/// General purpose color representation struct.
/// Allows for easy conversion between different formats.
/// </summary>
/// <remarks>
/// Consider using one of the storage types if you want efficient storage:
/// <see cref="LinearColor4"/>,
/// <see cref="SrgbColor4"/>
/// </remarks>
public record struct Color
{
    public static Color White => FromSrgb(1, 1, 1);
    public static Color Black => FromSrgb(0, 0, 0);
    public static Color Red => FromSrgb(1, 0, 0);
    public static Color Green => FromSrgb(0, 1, 0);
    public static Color Blue => FromSrgb(0, 0, 1);
    public static Color Yellow => FromSrgb(1, 1, 0);
    public static Color Cyan => FromSrgb(0, 1, 1);
    public static Color Magenta => FromSrgb(1, 0, 1);

    public static Color Transparent => TransparentWhite;
    public static Color TransparentWhite => FromSrgb(1, 1, 1, 0);
    public static Color TransparentBlack => FromSrgb(0, 0, 0, 0);

    private Vector4 color;
    private ColorType type;

    public ColorType Type
    {
        readonly get => type;
        set => type = value;
    }

    public Vector4 Value
    {
        readonly get => color;
        set => color = value;
    }

    public float X
    {
        readonly get => color.X;
        set => color.X = value;
    }

    public float Y
    {
        readonly get => color.Y;
        set => color.Y = value;
    }

    public float Z
    {
        readonly get => color.Z;
        set => color.Z = value;
    }

    public float W
    {
        readonly get => color.W;
        set => color.W = value;
    }

    public Color(Vector4 color, ColorType type)
    {
        this.color = color;
        this.type = type;
    }

    // Srgb

    public readonly Color Srgb => As(ColorType.Srgb);

    public static implicit operator Color(SrgbColor4 color)
    {
        return new Color(color.Value, ColorType.Srgb);
    }

    public static implicit operator SrgbColor4(Color color)
    {
        return new SrgbColor4(color.As(ColorType.Srgb).Value);
    }

    public static Color FromSrgb(Vector3 value)
    {
        return new Color(value.Xyz1(), ColorType.Srgb);
    }

    public static Color FromSrgb(Vector4 value)
    {
        return new Color(value, ColorType.Srgb);
    }

    public static Color FromSrgb(float r, float g, float b, float a = 1)
    {
        return new Color(new Vector4(r, g, b, a), ColorType.Srgb);
    }

    public static Color FromBytesSrgb(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        return new Color(new Vector4(r, g, b, a) / byte.MaxValue, ColorType.Srgb);
    }

    // Linear

    public readonly Color Linear => As(ColorType.Linear);

    public static implicit operator Color(LinearColor4 color)
    {
        return new Color(color.Value, ColorType.Linear);
    }

    public static implicit operator LinearColor4(Color color)
    {
        return new LinearColor4(color.As(ColorType.Linear).Value);
    }

    public static Color FromLinear(Vector3 value)
    {
        return new Color(value.Xyz1(), ColorType.Linear);
    }

    public static Color FromLinear(Vector4 value)
    {
        return new Color(value, ColorType.Linear);
    }

    public static Color FromLinear(float r, float g, float b, float a = 1)
    {
        return new Color(new Vector4(r, g, b, a), ColorType.Linear);
    }

    // Hsl

    public readonly Color Hsl => As(ColorType.Hsl);

    public static Color FromHsl(Vector3 value)
    {
        return new Color(value.Xyz1(), ColorType.Hsl);
    }

    public static Color FromHsl(Vector4 value)
    {
        return new Color(value, ColorType.Hsl);
    }

    public static Color FromHsl(float h, float s, float l, float a = 1)
    {
        return new Color(new Vector4(h, s, l, a), ColorType.Hsl);
    }

    // System.Drawing.Color

    public static implicit operator Color(DrawingColor color)
    {
        return FromDrawingColor(color);
    }

    public static implicit operator DrawingColor(Color color)
    {
        return color.ToDrawingColor();
    }

    public static Color FromDrawingColor(DrawingColor color)
    {
        var value = new Vector4(color.R, color.G, color.B, color.A) / byte.MaxValue;
        return new Color(value, ColorType.Srgb);
    }

    public readonly DrawingColor ToDrawingColor()
    {
        var value = Srgb.Value * byte.MaxValue;
        value = new Vector4(float.Round(value.X), float.Round(value.Y), float.Round(value.Z), float.Round(value.W));

        return DrawingColor.FromArgb((byte)value.W, (byte)value.X, (byte)value.Y, (byte)value.Z);
    }

    // Hex

    public static Color FromHex(string hex)
    {
        return new Color(M.HexToSrgb(hex), ColorType.Srgb);
    }

    public readonly string ToHex(bool includeAlpha = true)
    {
        var drawingColor = ToDrawingColor();

        return includeAlpha
            ? $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}{drawingColor.A:X2}"
            : $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}";
    }

    // Ansi

    public readonly string ToAnsi()
    {
        var drawingColor = ToDrawingColor();
        var value = (drawingColor.R << 16) | (drawingColor.G << 8) | (drawingColor.B << 0);

        return AnsiUtility.HexColorToAnsi(value);
    }

    public readonly string ToAnsiForeground()
    {
        return AnsiUtility.AnsiForeground(ToAnsi());
    }

    public readonly string ToAnsiBackground()
    {
        return AnsiUtility.AnsiBackground(ToAnsi());
    }

    // Conversions

    public readonly Color As(ColorType type)
    {
        return Convert(this, type);
    }

    private static Color Convert(Color color, ColorType targetType)
    {
        if (color.Type == targetType)
        {
            return color;
        }

        // Implementation note:
        // All conversions go through linear to keep things simple, but does lead to some inefficiencies

        // Convert to linear first
        var value = color.Value;
        switch (color.Type)
        {
            case ColorType.Linear:
            {
                break;
            }
            case ColorType.Srgb:
            {
                value = M.SrgbToLinear(value);
                break;
            }
            case ColorType.Hsl:
            {
                // Based on https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
                var h = M.Wrap(value.X, 0, 360); // [0, 360)
                var s = value.Y; // [0, 1]
                var l = value.Z; // [0, 1]
                var a = value.W; // [0, 1]

                var section = h / 60;
                var c = (1 - M.Abs(2 * l - 1)) * s;
                var x = c * (1 - M.Abs((section % 2) - 1));
                var m = l - (c / 2);

                var srgb = (int)section switch
                {
                    0 => new Vector4(c, x, 0, a),
                    1 => new Vector4(x, c, 0, a),
                    2 => new Vector4(0, c, x, a),
                    3 => new Vector4(0, x, c, a),
                    4 => new Vector4(x, 0, c, a),
                    5 => new Vector4(c, 0, x, a),
                    _ => throw new InvalidOperationException("Section value is out of range. This indicates an implementation error"),
                };

                srgb += new Vector4(m, m, m, 0);

                // This is inefficient if the requested type is Srgb
                value = M.SrgbToLinear(srgb);

                break;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(color.Type);
            }
        }

        // Convert to output type
        switch (targetType)
        {
            case ColorType.Linear:
            {
                break;
            }
            case ColorType.Srgb:
            {
                value = M.LinearToSrgb(value);
                break;
            }
            case ColorType.Hsl:
            {
                value = M.LinearToSrgb(value);

                var r = value.X;
                var g = value.Y;
                var b = value.Z;
                var a = value.W;

                var xMax = M.Max(M.Max(r, g), b);
                var xMin = M.Min(M.Min(r, g), b);

                var c = xMax - xMin;
                var l = (xMax + xMin) / 2;

                var h = 0f;
                if (c == 0)
                {
                    h = 0;
                }
                else if (xMax.Equals(r))
                {
                    h = 60 * (((g - b) / c) % 6);
                }
                else if (xMax.Equals(g))
                {
                    h = 60 * (((b - r) / c) + 2);
                }
                else if (xMax.Equals(b))
                {
                    h = 60 * (((r - g) / c) + 4);
                }

                var s = 0f;
                if (!l.Equals(0) || !l.Equals(1))
                {
                    s = (xMax - l) / M.Min(l, 1 - l);
                }

                value = new Vector4(h, s, l, a);

                break;
            }
            default:
            {
                throw ExceptionUtility.NotSupportedEnumValue(targetType);
            }
        }

        return new Color(value, targetType);
    }

    public readonly Color WithTypeOverride(ColorType type)
    {
        return new Color(Value, type);
    }

    // Comparisons

    public readonly bool ApproximatelyEquals(Color other, ColorType colorType = ColorType.Linear, float tolerance = 0.000001f)
    {
        return M.ApproximatelyEquals(As(colorType).Value, other.As(colorType).Value, tolerance);
    }

    // Operations

    public readonly Color AsPremultiplied()
    {
        return M.Premultiply(this);
    }

    public readonly Color WithAlpha(float a)
    {
        return this with
        {
            W = a,
        };
    }

    public readonly override string ToString()
    {
        return $"{Value} ({Type})";
    }

    // Predefined palettes

    /// <summary>
    /// Color palette based on Minecraft 1.12 dye colors.
    /// See https://minecraft.fandom.com/wiki/Dye
    /// </summary>
    public static class Dyes
    {
        public static Color White => FromHex("#F9FFFE");
        public static Color LightGray => FromHex("#9D9D97");
        public static Color Gray => FromHex("#474F52");
        public static Color Black => FromHex("#1D1D21");
        public static Color Brown => FromHex("#835432");
        public static Color Red => FromHex("#B02E26");
        public static Color Orange => FromHex("#F9801D");
        public static Color Yellow => FromHex("#FED83D");
        public static Color Lime => FromHex("#80C71F");
        public static Color Green => FromHex("#5E7C16");
        public static Color Cyan => FromHex("#169C9C");
        public static Color LightBlue => FromHex("#3AB3DA");
        public static Color Blue => FromHex("#3C44AA");
        public static Color Purple => FromHex("#8932B8");
        public static Color Magenta => FromHex("#C74EBD");
        public static Color Pink => FromHex("#F38BAA");
    }

    /// <summary>
    /// Color palette based on Tailwind 4 colors.
    /// See https://tailwindcss.com/docs/colors
    /// </summary>
    public static class Wind
    {
        public static Color Red50 => FromHex("#FEF2F2");
        public static Color Red100 => FromHex("#FFE2E2");
        public static Color Red200 => FromHex("#FFC9C9");
        public static Color Red300 => FromHex("#FFA2A2");
        public static Color Red400 => FromHex("#FF6467");
        public static Color Red500 => FromHex("#FB2C36");
        public static Color Red600 => FromHex("#E7000B");
        public static Color Red700 => FromHex("#C10007");
        public static Color Red800 => FromHex("#9F0712");
        public static Color Red900 => FromHex("#82181A");
        public static Color Red950 => FromHex("#460809");

        public static Color Orange50 => FromHex("#FFF7ED");
        public static Color Orange100 => FromHex("#FFEDD4");
        public static Color Orange200 => FromHex("#FFD6A7");
        public static Color Orange300 => FromHex("#FFB86A");
        public static Color Orange400 => FromHex("#FF8904");
        public static Color Orange500 => FromHex("#FF6900");
        public static Color Orange600 => FromHex("#F54900");
        public static Color Orange700 => FromHex("#CA3500");
        public static Color Orange800 => FromHex("#9F2D00");
        public static Color Orange900 => FromHex("#7E2A0C");
        public static Color Orange950 => FromHex("#441306");

        public static Color Amber50 => FromHex("#FFFBEB");
        public static Color Amber100 => FromHex("#FEF3C6");
        public static Color Amber200 => FromHex("#FEE685");
        public static Color Amber300 => FromHex("#FFD230");
        public static Color Amber400 => FromHex("#FFB900");
        public static Color Amber500 => FromHex("#FE9A00");
        public static Color Amber600 => FromHex("#E17100");
        public static Color Amber700 => FromHex("#BB4D00");
        public static Color Amber800 => FromHex("#973C00");
        public static Color Amber900 => FromHex("#7B3306");
        public static Color Amber950 => FromHex("#461901");

        public static Color Yellow50 => FromHex("#FEFCE8");
        public static Color Yellow100 => FromHex("#FEF9C2");
        public static Color Yellow200 => FromHex("#FFF085");
        public static Color Yellow300 => FromHex("#FFDF20");
        public static Color Yellow400 => FromHex("#FDC700");
        public static Color Yellow500 => FromHex("#F0B100");
        public static Color Yellow600 => FromHex("#D08700");
        public static Color Yellow700 => FromHex("#A65F00");
        public static Color Yellow800 => FromHex("#894B00");
        public static Color Yellow900 => FromHex("#733E0A");
        public static Color Yellow950 => FromHex("#432004");

        public static Color Lime50 => FromHex("#F7FEE7");
        public static Color Lime100 => FromHex("#ECFCCA");
        public static Color Lime200 => FromHex("#D8F999");
        public static Color Lime300 => FromHex("#BBF451");
        public static Color Lime400 => FromHex("#9AE600");
        public static Color Lime500 => FromHex("#7CCF00");
        public static Color Lime600 => FromHex("#5EA500");
        public static Color Lime700 => FromHex("#497D00");
        public static Color Lime800 => FromHex("#3C6300");
        public static Color Lime900 => FromHex("#35530E");
        public static Color Lime950 => FromHex("#192E03");

        public static Color Green50 => FromHex("#F0FDF4");
        public static Color Green100 => FromHex("#DCFCE7");
        public static Color Green200 => FromHex("#B9F8CF");
        public static Color Green300 => FromHex("#7BF1A8");
        public static Color Green400 => FromHex("#05DF72");
        public static Color Green500 => FromHex("#00C950");
        public static Color Green600 => FromHex("#00A63E");
        public static Color Green700 => FromHex("#008236");
        public static Color Green800 => FromHex("#016630");
        public static Color Green900 => FromHex("#0D542B");
        public static Color Green950 => FromHex("#032E15");

        public static Color Emerald50 => FromHex("#ECFDF5");
        public static Color Emerald100 => FromHex("#D0FAE5");
        public static Color Emerald200 => FromHex("#A4F4CF");
        public static Color Emerald300 => FromHex("#5EE9B5");
        public static Color Emerald400 => FromHex("#00D492");
        public static Color Emerald500 => FromHex("#00BC7D");
        public static Color Emerald600 => FromHex("#009966");
        public static Color Emerald700 => FromHex("#007A55");
        public static Color Emerald800 => FromHex("#006045");
        public static Color Emerald900 => FromHex("#004F3B");
        public static Color Emerald950 => FromHex("#002C22");

        public static Color Teal50 => FromHex("#F0FDFA");
        public static Color Teal100 => FromHex("#CBFBF1");
        public static Color Teal200 => FromHex("#96F7E4");
        public static Color Teal300 => FromHex("#46ECD5");
        public static Color Teal400 => FromHex("#00D5BE");
        public static Color Teal500 => FromHex("#00BBA7");
        public static Color Teal600 => FromHex("#009689");
        public static Color Teal700 => FromHex("#00786F");
        public static Color Teal800 => FromHex("#005F5A");
        public static Color Teal900 => FromHex("#0B4F4A");
        public static Color Teal950 => FromHex("#022F2E");

        public static Color Cyan50 => FromHex("#ECFEFF");
        public static Color Cyan100 => FromHex("#CEFAFE");
        public static Color Cyan200 => FromHex("#A2F4FD");
        public static Color Cyan300 => FromHex("#53EAFD");
        public static Color Cyan400 => FromHex("#00D3F2");
        public static Color Cyan500 => FromHex("#00B8DB");
        public static Color Cyan600 => FromHex("#0092B8");
        public static Color Cyan700 => FromHex("#007595");
        public static Color Cyan800 => FromHex("#005F78");
        public static Color Cyan900 => FromHex("#104E64");
        public static Color Cyan950 => FromHex("#053345");

        public static Color Sky50 => FromHex("#F0F9FF");
        public static Color Sky100 => FromHex("#DFF2FE");
        public static Color Sky200 => FromHex("#B8E6FE");
        public static Color Sky300 => FromHex("#74D4FF");
        public static Color Sky400 => FromHex("#00BCFF");
        public static Color Sky500 => FromHex("#00A6F4");
        public static Color Sky600 => FromHex("#0084D1");
        public static Color Sky700 => FromHex("#0069A8");
        public static Color Sky800 => FromHex("#00598A");
        public static Color Sky900 => FromHex("#024A70");
        public static Color Sky950 => FromHex("#052F4A");

        public static Color Blue50 => FromHex("#EFF6FF");
        public static Color Blue100 => FromHex("#DBEAFE");
        public static Color Blue200 => FromHex("#BEDBFF");
        public static Color Blue300 => FromHex("#8EC5FF");
        public static Color Blue400 => FromHex("#51A2FF");
        public static Color Blue500 => FromHex("#2B7FFF");
        public static Color Blue600 => FromHex("#155DFC");
        public static Color Blue700 => FromHex("#1447E6");
        public static Color Blue800 => FromHex("#193CB8");
        public static Color Blue900 => FromHex("#1C398E");
        public static Color Blue950 => FromHex("#162456");

        public static Color Indigo50 => FromHex("#EEF2FF");
        public static Color Indigo100 => FromHex("#E0E7FF");
        public static Color Indigo200 => FromHex("#C6D2FF");
        public static Color Indigo300 => FromHex("#A3B3FF");
        public static Color Indigo400 => FromHex("#7C86FF");
        public static Color Indigo500 => FromHex("#615FFF");
        public static Color Indigo600 => FromHex("#4F39F6");
        public static Color Indigo700 => FromHex("#432DD7");
        public static Color Indigo800 => FromHex("#372AAC");
        public static Color Indigo900 => FromHex("#312C85");
        public static Color Indigo950 => FromHex("#1E1A4D");

        public static Color Violet50 => FromHex("#F5F3FF");
        public static Color Violet100 => FromHex("#EDE9FE");
        public static Color Violet200 => FromHex("#DDD6FF");
        public static Color Violet300 => FromHex("#C4B4FF");
        public static Color Violet400 => FromHex("#A684FF");
        public static Color Violet500 => FromHex("#8E51FF");
        public static Color Violet600 => FromHex("#7F22FE");
        public static Color Violet700 => FromHex("#7008E7");
        public static Color Violet800 => FromHex("#5D0EC0");
        public static Color Violet900 => FromHex("#4D179A");
        public static Color Violet950 => FromHex("#2F0D68");

        public static Color Purple50 => FromHex("#FAF5FF");
        public static Color Purple100 => FromHex("#F3E8FF");
        public static Color Purple200 => FromHex("#E9D4FF");
        public static Color Purple300 => FromHex("#DAB2FF");
        public static Color Purple400 => FromHex("#C27AFF");
        public static Color Purple500 => FromHex("#AD46FF");
        public static Color Purple600 => FromHex("#9810FA");
        public static Color Purple700 => FromHex("#8200DB");
        public static Color Purple800 => FromHex("#6E11B0");
        public static Color Purple900 => FromHex("#59168B");
        public static Color Purple950 => FromHex("#3C0366");

        public static Color Fuchsia50 => FromHex("#FDF4FF");
        public static Color Fuchsia100 => FromHex("#FAE8FF");
        public static Color Fuchsia200 => FromHex("#F6CFFF");
        public static Color Fuchsia300 => FromHex("#F4A8FF");
        public static Color Fuchsia400 => FromHex("#ED6AFF");
        public static Color Fuchsia500 => FromHex("#E12AFB");
        public static Color Fuchsia600 => FromHex("#C800DE");
        public static Color Fuchsia700 => FromHex("#A800B7");
        public static Color Fuchsia800 => FromHex("#8A0194");
        public static Color Fuchsia900 => FromHex("#721378");
        public static Color Fuchsia950 => FromHex("#4B004F");

        public static Color Pink50 => FromHex("#FDF2F8");
        public static Color Pink100 => FromHex("#FCE7F3");
        public static Color Pink200 => FromHex("#FCCEE8");
        public static Color Pink300 => FromHex("#FDA5D5");
        public static Color Pink400 => FromHex("#FB64B6");
        public static Color Pink500 => FromHex("#F6339A");
        public static Color Pink600 => FromHex("#E60076");
        public static Color Pink700 => FromHex("#C6005C");
        public static Color Pink800 => FromHex("#A3004C");
        public static Color Pink900 => FromHex("#861043");
        public static Color Pink950 => FromHex("#510424");

        public static Color Rose50 => FromHex("#FFF1F2");
        public static Color Rose100 => FromHex("#FFE4E6");
        public static Color Rose200 => FromHex("#FFCCD3");
        public static Color Rose300 => FromHex("#FFA1AD");
        public static Color Rose400 => FromHex("#FF637E");
        public static Color Rose500 => FromHex("#FF2056");
        public static Color Rose600 => FromHex("#EC003F");
        public static Color Rose700 => FromHex("#C70036");
        public static Color Rose800 => FromHex("#A50036");
        public static Color Rose900 => FromHex("#8B0836");
        public static Color Rose950 => FromHex("#4D0218");

        public static Color Slate50 => FromHex("#F8FAFC");
        public static Color Slate100 => FromHex("#F1F5F9");
        public static Color Slate200 => FromHex("#E2E8F0");
        public static Color Slate300 => FromHex("#CAD5E2");
        public static Color Slate400 => FromHex("#90A1B9");
        public static Color Slate500 => FromHex("#62748E");
        public static Color Slate600 => FromHex("#45556C");
        public static Color Slate700 => FromHex("#314158");
        public static Color Slate800 => FromHex("#1D293D");
        public static Color Slate900 => FromHex("#0F172B");
        public static Color Slate950 => FromHex("#020618");

        public static Color Gray50 => FromHex("#F9FAFB");
        public static Color Gray100 => FromHex("#F3F4F6");
        public static Color Gray200 => FromHex("#E5E7EB");
        public static Color Gray300 => FromHex("#D1D5DC");
        public static Color Gray400 => FromHex("#99A1AF");
        public static Color Gray500 => FromHex("#6A7282");
        public static Color Gray600 => FromHex("#4A5565");
        public static Color Gray700 => FromHex("#364153");
        public static Color Gray800 => FromHex("#1E2939");
        public static Color Gray900 => FromHex("#101828");
        public static Color Gray950 => FromHex("#030712");

        public static Color Zinc50 => FromHex("#FAFAFA");
        public static Color Zinc100 => FromHex("#F4F4F5");
        public static Color Zinc200 => FromHex("#E4E4E7");
        public static Color Zinc300 => FromHex("#D4D4D8");
        public static Color Zinc400 => FromHex("#9F9FA9");
        public static Color Zinc500 => FromHex("#71717B");
        public static Color Zinc600 => FromHex("#52525C");
        public static Color Zinc700 => FromHex("#3F3F46");
        public static Color Zinc800 => FromHex("#27272A");
        public static Color Zinc900 => FromHex("#18181B");
        public static Color Zinc950 => FromHex("#09090B");

        public static Color Neutral50 => FromHex("#FAFAFA");
        public static Color Neutral100 => FromHex("#F5F5F5");
        public static Color Neutral200 => FromHex("#E5E5E5");
        public static Color Neutral300 => FromHex("#D4D4D4");
        public static Color Neutral400 => FromHex("#A1A1A1");
        public static Color Neutral500 => FromHex("#737373");
        public static Color Neutral600 => FromHex("#525252");
        public static Color Neutral700 => FromHex("#404040");
        public static Color Neutral800 => FromHex("#262626");
        public static Color Neutral900 => FromHex("#171717");
        public static Color Neutral950 => FromHex("#0A0A0A");

        public static Color Stone50 => FromHex("#FAFAF9");
        public static Color Stone100 => FromHex("#F5F5F4");
        public static Color Stone200 => FromHex("#E7E5E4");
        public static Color Stone300 => FromHex("#D6D3D1");
        public static Color Stone400 => FromHex("#A6A09B");
        public static Color Stone500 => FromHex("#79716B");
        public static Color Stone600 => FromHex("#57534D");
        public static Color Stone700 => FromHex("#44403B");
        public static Color Stone800 => FromHex("#292524");
        public static Color Stone900 => FromHex("#1C1917");
        public static Color Stone950 => FromHex("#0C0A09");
    }
}

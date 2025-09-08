namespace Exanite.Core.Utilities;

public static class AnsiUtility
{
    /// <summary>
    /// Converts a hex color to an ansi color escape code.
    /// </summary>
    /// <remarks>
    /// The color is expected to be defined as a C# hexadecimal literal.
    /// For example: var color = 0xff0000
    /// </remarks>
    public static string HexColorToAnsi(int color)
    {
        return $"{(color >> 16) & 0b11111111};{(color >> 8) & 0b11111111};{(color >> 0) & 0b11111111}";
    }

    /// <summary>
    /// Creates an ansi foreground escape code for the specified color.
    /// </summary>
    public static string AnsiForeground(string ansiColor)
    {
        return $"\u001B[38;2;{ansiColor}m";
    }

    /// <summary>
    /// Creates an ansi foreground escape code for the specified color.
    /// </summary>
    public static string AnsiForeground(int color)
    {
        return AnsiForeground(HexColorToAnsi(color));
    }

    /// <summary>
    /// Creates an ansi background escape code for the specified color.
    /// </summary>
    public static string AnsiBackground(string ansiColor)
    {
        return $"\u001B[48;2;{ansiColor}m";
    }

    /// <summary>
    /// Creates an ansi background escape code for the specified color.
    /// </summary>
    public static string AnsiBackground(int color)
    {
        return AnsiBackground(HexColorToAnsi(color));
    }

    /// <summary>
    /// Creates an ansi reset escape code.
    /// </summary>
    public static string AnsiReset()
    {
        return "\u001B[0m";
    }
}

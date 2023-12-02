using System;

namespace Exanite.Core.Numbers
{
    /// <summary>
    /// How the class <see cref="LargeNumber"/> display the number.
    /// </summary>
    [Serializable]
    public enum NumDisplayFormat
    {
        /// <summary>
        /// Displays the number in scientific notation (123.456 E+6).
        /// </summary>
        Scientific,

        /// <summary>
        /// Displays the number as an abbreviation (123.456 M).
        /// </summary>
        Short,

        /// <summary>
        /// Displays the whole name for the number (123.456 Million).
        /// </summary>
        Long,
    }
}

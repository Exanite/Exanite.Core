// Originally from https://gist.github.com/oxysoft/66fe16fd12f1402232e8a0c770f3a89e

using System;

namespace Exanite.Core.OdinInspector
{
    /// <summary>
    /// Draw the properties with a darker background and
    /// borders, optionally.
    /// </summary>
    public class DarkBoxAttribute : Attribute
    {
        /// <summary>
        /// Dark
        /// </summary>
        public bool WithBorders { get; set; }
    }

    public class ColorBoxAttribute : Attribute {}
}

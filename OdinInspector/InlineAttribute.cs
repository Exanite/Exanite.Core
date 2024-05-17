// Originally from https://gist.github.com/oxysoft/acff4ebbab474b3d5cb785890b3cac78

using System;

namespace Exanite.Core.OdinInspector
{
    public enum InlineLabelType
    {
        /// <summary>
        /// Do not draw a label.
        /// </summary>
        Hidden = 0,

        /// <summary>
        /// Draws a label as a prefix.
        /// </summary>
        Prefix = 1,

        /// <summary>
        /// Draws a label above.
        /// </summary>
        Above = 2,

        /// <summary>
        /// Draws a bolded label above.
        /// </summary>
        BoldedAbove = 3,
    }

    [AttributeUsage(AttributeTargets.All)]
    public class InlineAttribute : Attribute
    {
        public InlineLabelType Label { get; set; }
    }

    /// <summary>
    /// Inline with a box surrounding the properties, like BoxGroup.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class InlineBoxAttribute : Attribute {}

    /// Inline with a foldout box surrounding the properties, like FoldoutGroup.
    [AttributeUsage(AttributeTargets.All)]
    public class InlineFoldoutAttribute : Attribute {}
}

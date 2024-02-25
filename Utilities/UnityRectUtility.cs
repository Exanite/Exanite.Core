#if UNITY_2021_3_OR_NEWER
using UnityEngine;

namespace Exanite.Core.Utilities
{
    public static class UnityRectUtility
    {
        public static Rect WithInset(this Rect rect, float inset)
        {
            rect.xMin += inset;
            rect.yMin += inset;

            rect.xMax -= inset;
            rect.yMax -= inset;

            return rect;
        }
    }
}
#endif

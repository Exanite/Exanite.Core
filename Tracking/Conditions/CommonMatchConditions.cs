using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Exanite.Core.Tracking.Conditions
{
    public static class CommonMatchConditions
    {
        /// <summary>
        /// Never automatically register objects. This is useful for manually registering objects.
        /// </summary>
        public static bool NoOp<T>(GameObject gameObject, [NotNullWhen(true)] out T? narrowedValue) where T : class
        {
            narrowedValue = default;
            return false;
        }

        /// <summary>
        /// Automatically register components on a GameObject.
        /// </summary>
        public static bool ComponentOnGameObject<T>(GameObject gameObject, [NotNullWhen(true)] out T? narrowedValue) where T : class
        {
            if (gameObject.TryGetComponent(out T display))
            {
                narrowedValue = display;
                return true;
            }

            narrowedValue = null;
            return false;
        }
    }
}

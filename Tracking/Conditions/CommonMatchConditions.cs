using UnityEngine;

namespace Exanite.Core.Tracking.Conditions
{
    public static class CommonMatchConditions
    {
        public static bool ComponentOnGameObject<T>(GameObject gameObject, out T narrowedValue) where T : class
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

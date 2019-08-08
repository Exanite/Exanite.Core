using UnityEngine;

namespace Exanite.Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component)
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Collider collider) where T : Component
        {
            T component = collider.GetComponent<T>();

            if (component)
            {
                return component;
            }

            return collider.gameObject.AddComponent<T>();
        }
    }
}

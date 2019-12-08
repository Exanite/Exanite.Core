using UnityEngine;

namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="GameObject"/>s
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets or adds a component
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component)
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets or adds a component
        /// </summary>
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

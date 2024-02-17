#if UNITY_2021_3_OR_NEWER
using UnityEngine;

namespace Exanite.Core.Utilities
{
    public static class GameObjectUtility
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();

            if (component)
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// Gets a <see cref="Component"/> if it exists, throws a
        /// <see cref="MissingComponentException"/> if it does not.
        /// </summary>
        /// <exception cref="MissingComponentException"/>
        public static T GetRequiredComponent<T>(this GameObject gameObject) where T : class
        {
            var component = gameObject.GetComponent<T>() as Component;

            if (component)
            {
                return component as T;
            }

            throw new MissingComponentException($"There is no {typeof(T).Name} attached to the '{gameObject.name} game object'");
        }

        /// <summary>
        /// Gets a <see cref="Component"/> if it exists, throws a
        /// <see cref="MissingComponentException"/> if it does not.
        /// </summary>
        /// <exception cref="MissingComponentException"/>
        public static T GetRequiredComponent<T>(this Component component) where T : class
        {
            return component.gameObject.GetRequiredComponent<T>();
        }
    }
}
#endif

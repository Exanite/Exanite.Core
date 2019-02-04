using System;
using System.Collections;
using UnityEngine;

namespace Exanite.Utility
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if an <see cref="Array"/> is <see langword="null"/> or empty
        /// </summary>
        /// <param name="array"><see cref="Array"/> to check</param>
        /// <returns>Is the <see cref="Array"/> <see langword="null"/> or empty</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        /// <summary>
        /// Checks if an <see cref="IList"/> is <see langword="null"/> or empty
        /// </summary>
        /// <param name="array"><see cref="IList"/> to check</param>
        /// <returns>Is the <see cref="IList"/> <see langword="null"/> or empty</returns>
        public static bool IsNullOrEmpty(this IList list)
        {
            return (list == null || list.Count == 0);
        }

        /// <summary>
        /// Gets or adds a <see cref="Component"/>
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <see cref="Component"/> to add</typeparam>
        /// <param name="gameObject"><see cref="GameObject"/> to perform this method on</param>
        /// <returns>Component of <see langword="type"/> T</returns>
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
        /// Gets or adds a <see cref="Component"/>
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <see cref="Component"/> to add</typeparam>
        /// <param name="collider"><see cref="Collider"/> to perform this method on</param>
        /// <returns>Component of <see langword="type"/> T</returns>
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
#if UNITY_2021_3_OR_NEWER
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Utility class for miscellaneous Unity methods.
    /// </summary>
    public static class UnityUtility
    {
        /// <summary>
        /// Calls Destroy in Play mode and calls DestroyImmediate in Edit
        /// mode. Unsafe because this can destroy assets.
        /// </summary>
        public static void UnsafeDestroy(Object obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }

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
            if (gameObject.GetComponent<T>() is {} typedComponent)
            {
                return typedComponent;
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

        public static GameObject Instantiate(this Scene scene, GameObject original)
        {
            return scene.Instantiate(original, Vector3.zero, Quaternion.identity);
        }

        public static T Instantiate<T>(this Scene scene, T original) where T : Component
        {
            return scene.Instantiate(original, Vector3.zero, Quaternion.identity);
        }

        public static GameObject Instantiate(this Scene scene, GameObject original, Vector3 position, Quaternion rotation)
        {
            var gameObject = Object.Instantiate(original, position, rotation);
            SceneManager.MoveGameObjectToScene(gameObject, scene);

            return gameObject;
        }

        public static T Instantiate<T>(this Scene scene, T original, Vector3 position, Quaternion rotation) where T : Component
        {
            var component = Object.Instantiate(original, position, rotation);
            SceneManager.MoveGameObjectToScene(component.gameObject, scene);

            return component;
        }

        public static GameObject InstantiateNew(this Scene scene, string name)
        {
            var gameObject = new GameObject(name);
            SceneManager.MoveGameObjectToScene(gameObject, scene);

            return gameObject;
        }
    }
}
#endif

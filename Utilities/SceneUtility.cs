#if UNITY_2021_3_OR_NEWER
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exanite.Core.Utilities
{
    public static class SceneUtility
    {
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

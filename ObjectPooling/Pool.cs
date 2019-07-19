using UnityEngine;

namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Simplifies calling <see cref="GameObjectPool"/> methods
    /// </summary>
    public static class Pool
    {
        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            return GameObjectPool.Instance.Spawn(prefab, prefab.transform.position, prefab.transform.rotation, parent);
        }

        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
        {
            return GameObjectPool.Instance.Spawn(prefab, position, prefab.transform.rotation, parent);
        }

        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return GameObjectPool.Instance.Spawn(prefab, position, rotation, parent);
        }

        /// <summary>
        /// Despawns a pooled prefab and returns it to the pool
        /// </summary>
        public static void Despawn(GameObject gameObjectToDespawn)
        {
            GameObjectPool.Instance.Despawn(gameObjectToDespawn);
        }

        /// <summary>
        /// Creates a new pool
        /// </summary>
        public static void CreatePool(GameObject prefab, int poolSize = 1)
        {
            GameObjectPool.Instance.CreatePool(prefab, poolSize);
        }

        /// <summary>
        /// Expands an existing pool or creates a new one if there isn't an existing one
        /// </summary>
        public static void ExpandPool(GameObject prefab, int amount = 1)
        {
            GameObjectPool.Instance.ExpandPool(prefab, amount);
        }
    }
}

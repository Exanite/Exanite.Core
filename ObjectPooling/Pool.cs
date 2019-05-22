using UnityEngine;
using Exanite.Core.ObjectPooling.Internal;

namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Simplifies calling <see cref="PoolController"/> methods
    /// </summary>
    public static class Pool
    {
        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        /// <param name="prefab">Prefab to spawn</param>
        /// <param name="parent">Parent of spawned prefab</param>
        /// <returns>The spawned prefab</returns>
        public static GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            return PoolController.Instance.Spawn(prefab, prefab.transform.position, prefab.transform.rotation, parent);
        }

        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        /// <param name="prefab">Prefab to spawn</param>
        /// <param name="position">position of spawned prefab</param>
        /// <param name="parent">Parent of spawned prefab</param>
        /// <returns>The spawned prefab</returns>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
        {
            return PoolController.Instance.Spawn(prefab, position, prefab.transform.rotation, parent);
        }

        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        /// <param name="prefab">Prefab to spawn</param>
        /// <param name="position">position of spawned prefab</param>
        /// <param name="rotation">Rotation of spawned prefab</param>
        /// <param name="parent">Parent of spawned prefab</param>
        /// <returns>The spawned prefab</returns>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return PoolController.Instance.Spawn(prefab, position, rotation, parent);
        }

        /// <summary>
        /// Despawns a pooled prefab and returns it to the pool
        /// </summary>
        /// <param name="gameObjectToDespawn"></param>
        public static void Despawn(GameObject gameObjectToDespawn)
        {
            PoolController.Instance.Despawn(gameObjectToDespawn);
        }

        /// <summary>
        /// Creates a new pool
        /// </summary>
        /// <param name="prefab">Prefab to pool</param>
        /// <param name="poolSize">Size of the pool</param>
        /// <param name="overrideExisting">Should this replace existing pools</param>
        public static void CreatePool(GameObject prefab, int poolSize = 1, bool overrideExisting = false)
        {
            PoolController.Instance.CreatePool(prefab, poolSize, overrideExisting);
        }

        /// <summary>
        /// Expands an existing pool or creates a new one if there isn't an existing one
        /// </summary>
        /// <param name="prefab">Prefab pool to expand</param>
        /// <param name="amount">Amount to expand the pool by</param>
        public static void ExpandPool(GameObject prefab, int amount = 1)
        {
            PoolController.Instance.ExpandPool(prefab, amount);
        }
    }
}

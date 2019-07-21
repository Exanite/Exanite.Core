using Exanite.Core.Extensions;
using Exanite.Core.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.Core.ObjectPooling
{
    [DefaultExecutionOrder(-1000)]
    public class GameObjectPool : SingletonBehaviour<GameObjectPool>
    {
        public List<AutoCreatedPools> PoolsToCreate = new List<AutoCreatedPools>();

        protected Dictionary<int, Pool> pools;
        protected bool isDirty = false;
        protected List<GameObject> spawnedGameObjects;

        protected static GameObjectPool instance;

        protected virtual void Awake()
        {
            pools = new Dictionary<int, Pool>();
            spawnedGameObjects = new List<GameObject>();

            foreach (AutoCreatedPools poolToCreate in PoolsToCreate)
            {
                CreatePool(poolToCreate.prefab, poolToCreate.amount);
            }
        }

        protected virtual void Update()
        {
            if (isDirty)
            {
                isDirty = false;

                foreach (GameObject spawnedGameObject in spawnedGameObjects)
                {
                    foreach (IPoolableGameObject iPoolable in spawnedGameObject.GetComponentsInChildren<IPoolableGameObject>())
                    {
                        iPoolable.OnSpawn();
                    }
                }

                spawnedGameObjects.Clear();
            }
        }

        /// <summary>
        /// Creates a new pool
        /// </summary>
        public virtual void CreatePool(GameObject prefab, int poolSize = 10)
        {
            int poolKey = prefab.GetInstanceID();

            Pool pool;
            if (pools.ContainsKey(poolKey))
            {
                pool = pools[poolKey];
            }
            else
            {
                pool = pools[poolKey] = new Pool(prefab);
            }

            ExpandPool(prefab, poolSize - pool.Queue.Count);
        }

        /// <summary>
        /// Expands an existing pool or creates a new one if there isn't an existing one
        /// </summary>
        public virtual void ExpandPool(GameObject prefab, int amount = 5)
        {
            int poolKey = prefab.GetInstanceID();

            if (pools.ContainsKey(poolKey))
            {
                for (int i = 0; i < amount; i++)
                {
                    GameObject poolObject = Instantiate(prefab);
                    poolObject.SetActive(false);
                    poolObject.transform.SetParent(transform, true);

                    PoolInstanceID poolObjectID = poolObject.GetOrAddComponent<PoolInstanceID>();
                    poolObjectID.instanceID = poolKey;
                    poolObjectID.originalPrefab = prefab;

                    pools[poolKey].Queue.Enqueue(poolObject);
                }
            }
            else
            {
                CreatePool(prefab, amount);
            }
        }

        /// <summary>
        /// Spawns a pooled prefab
        /// </summary>
        public virtual GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();

            if (pools.ContainsKey(poolKey)) // Spawn object
            {
                if (pools[poolKey].Queue.Count <= 0) // If empty
                {
                    ExpandPool(prefab);
                }

                GameObject poolObject = pools[poolKey].Queue.Dequeue();

                poolObject.transform.position = position; // Set object transforms
                poolObject.transform.rotation = rotation;
                poolObject.transform.SetParent(parent, true);
                poolObject.SetActive(true);

                isDirty = true; // Add it to the list of objects that need OnSpawn called
                spawnedGameObjects.Add(poolObject);

                return poolObject;
            }
            else // Create pool and retry
            {
                CreatePool(prefab);
                return Spawn(prefab, position, rotation, parent);
            }
        }

        /// <summary>
        /// Despawns a pooled prefab and returns it to the pool
        /// </summary>
        public virtual void Despawn(GameObject gameObjectToDespawn)
        {
            int poolKey = (int)gameObjectToDespawn.GetComponent<PoolInstanceID>()?.instanceID;

            if (pools.ContainsKey(poolKey))
            {
                foreach (IPoolableGameObject iPoolable in gameObjectToDespawn.GetComponentsInChildren<IPoolableGameObject>())
                {
                    iPoolable.OnDespawn();
                }

                gameObjectToDespawn.SetActive(false);
                gameObjectToDespawn.transform.SetParent(transform);
                pools[poolKey].Queue.Enqueue(gameObjectToDespawn);
            }
            else
            {
                Destroy(gameObjectToDespawn);
            }
        }

        /// <summary>
        /// Holds pool data
        /// </summary>
        public class Pool
        {
            public GameObject Prefab;
            public Queue<GameObject> Queue = new Queue<GameObject>();

            public Pool(GameObject prefab)
            {
                Prefab = prefab;
            }
        }

        [Serializable]
        public class AutoCreatedPools
        {
            public GameObject prefab;
            public int amount;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.ObjectPooling.Internal
{
    /// <summary>
    /// Main class for object pooling
    /// </summary>
    public class PoolController : MonoBehaviour 
    {
        /// <summary>
        /// Pools to create on <see cref="Awake"/>
        /// </summary>
        public List<AutoCreatedPools> poolsToCreate = new List<AutoCreatedPools>();

        /// <summary>
        /// Should this output debug messages?
        /// </summary>
        public bool debugMode = false;

        /// <summary>
        /// Dictionary of pools
        /// </summary>
        protected Dictionary<int, Pool> _poolDictionary;
        /// <summary>
        /// Did anything get spawned this frame?
        /// </summary>
        protected bool _isDirty = false;
        /// <summary>
        /// GameObjects that spawned this frame and requires their <see cref="IPoolable.OnSpawn"/> to be called
        /// </summary>
        protected List<GameObject> _spawnedGameObjects;

        private static PoolController instance;

        /// <summary>
        /// Singleton instance of <see cref="PoolController"/>
        /// </summary>
        public static PoolController Instance
        {
            get
            {
                if(!instance)
                {
                    new GameObject("_ObjectPool").AddComponent<PoolController>();
                }
                return instance;
            }
        }

        protected virtual void Awake() 
        {
            if(!instance)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning($"There is already a {GetType()} in the scene.");
                Destroy(this);
            }
            
            _poolDictionary = new Dictionary<int, Pool>();
            _spawnedGameObjects = new List<GameObject>();

            foreach(AutoCreatedPools poolToCreate in poolsToCreate)
            {
                CreatePool(poolToCreate.prefab, poolToCreate.amount, true);
            }
        }

        protected virtual void LateUpdate() 
        {
            if(_isDirty)
            {
                foreach(GameObject spawnedGameObject in _spawnedGameObjects)
                {
                    foreach(IPoolable iPoolable in spawnedGameObject.GetComponentsInChildren<IPoolable>())
                    {
                        iPoolable.OnSpawn();
                    }
                }

                _isDirty = false;
                _spawnedGameObjects.Clear();
            }
        }

        /// <summary>
        /// Creates a new pool
        /// </summary>
        /// <param name="prefab">Prefab to pool</param>
        /// <param name="poolSize">Size of the pool</param>
        /// <param name="overrideExisting">Should this replace existing pools</param>
        public virtual void CreatePool(GameObject prefab, int poolSize = 10, bool overrideExisting = false)
        {
            int poolKey = prefab.GetInstanceID();

            if(!_poolDictionary.ContainsKey(poolKey) || overrideExisting)
            {
                if(debugMode) Debug.Log($"Creating object pool for {prefab.name} with InstanceID of {poolKey}");

                Queue<GameObject> oldQueue;
                if(_poolDictionary.ContainsKey(poolKey)) 
                {
                    oldQueue = _poolDictionary[poolKey].queue;
                    _poolDictionary.Remove(poolKey);
                }
                else
                {
                    oldQueue = new Queue<GameObject>();
                }

                _poolDictionary.Add(poolKey, new Pool(prefab));
                if(oldQueue.Count > 0)
                {
                    foreach(GameObject gameObject in oldQueue)
                    {
                        _poolDictionary[poolKey].queue.Enqueue(gameObject);
                    }
                }

                ExpandPool(prefab, poolSize - oldQueue.Count);
            }
        }

        /// <summary>
        /// Expands an existing pool or creates a new one if there isn't an existing one
        /// </summary>
        /// <param name="prefab">Prefab pool to expand</param>
        /// <param name="amount">Amount to expand the pool by</param>
        public virtual void ExpandPool(GameObject prefab, int amount = 5)
        {
            int poolKey = prefab.GetInstanceID();

            if(_poolDictionary.ContainsKey(poolKey))
            {
                bool doesHaveIDComponent = false;
                if(prefab.GetComponent<PoolInstanceID>())
                {
                    doesHaveIDComponent = true;
                }

                for (int i = 0; i < amount; i++)
                {
                    GameObject poolObject = Instantiate(prefab) as GameObject;
                    poolObject.SetActive(false);
                    poolObject.transform.SetParent(transform);
                    if(!doesHaveIDComponent)
                    {
                        poolObject.AddComponent<PoolInstanceID>();
                    }
                    PoolInstanceID poolObjectID = poolObject.GetComponent<PoolInstanceID>();
                    poolObjectID.instanceID = poolKey;
                    poolObjectID.originalPrefab = prefab;

                    _poolDictionary[poolKey].queue.Enqueue(poolObject);
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
        /// <param name="prefab">Prefab to spawn</param>
        /// <param name="position">position of spawned prefab</param>
        /// <param name="rotation">Rotation of spawned prefab</param>
        /// <param name="parent">Parent of spawned prefab</param>
        /// <returns>The spawned prefab</returns>
        public virtual GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();

            if(_poolDictionary.ContainsKey(poolKey)) //Spawn object
            {
                if(_poolDictionary[poolKey].queue.Count <= 0) // If empty
                {
                    ExpandPool(prefab);
                }

                GameObject poolObject = _poolDictionary[poolKey].queue.Dequeue();
                
                poolObject.transform.position = position; // Set object transforms
                poolObject.transform.rotation = rotation;
                poolObject.transform.SetParent(parent, true);
                poolObject.SetActive(true);

                _isDirty = true; // Add it to the list of objects that need OnSpawn called
                _spawnedGameObjects.Add(poolObject);

                return poolObject;
            }
            else //Create pool and retry
            {
                if(debugMode) Debug.Log($"Prefab {prefab.name} does not have a pool yet, creating now.");
                CreatePool(prefab);
                return Spawn(prefab, position, rotation, parent);
            }
        }

        /// <summary>
        /// Despawns a pooled prefab and returns it to the pool
        /// </summary>
        /// <param name="gameObjectToDespawn"></param>
        public virtual void Despawn(GameObject gameObjectToDespawn)
        {
            int poolKey = gameObjectToDespawn.GetComponent<PoolInstanceID>().instanceID;

            if(_poolDictionary.ContainsKey(poolKey))
            {
                foreach(IPoolable iPoolable in gameObjectToDespawn.GetComponentsInChildren<IPoolable>())
                {
                    iPoolable.OnDespawn();
                }

                gameObjectToDespawn.SetActive(false);
                gameObjectToDespawn.transform.SetParent(transform);
                _poolDictionary[poolKey].queue.Enqueue(gameObjectToDespawn);
            }
            else
            {
                if(debugMode) Debug.LogWarning($"{gameObjectToDespawn.name} does not have a corresponding pool and will be destroyed instead.");
                Destroy(gameObjectToDespawn);
            }
        }

        /// <summary>
        /// Holds pool data
        /// </summary>
        public class Pool
        {
            /// <summary>
            /// Pooled prefab
            /// </summary>
            public GameObject prefab;
            /// <summary>
            /// Pool queue
            /// </summary>
            public Queue<GameObject> queue;

            /// <summary>
            /// Creates a new pool
            /// </summary>
            /// <param name="_prefab"></param>
            public Pool(GameObject _prefab)
            {
                prefab = _prefab;
                queue = new Queue<GameObject>();
            }
        }

        /// <summary>
        /// Pools to auto-create (For editor use)
        /// </summary>
        [System.Serializable]
        public class AutoCreatedPools
        {
            public GameObject prefab;
            public int amount;
        }
    }
}
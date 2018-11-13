using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exanite.ObjectPooling;

namespace Exanite.ObjectPooling.Internal
{
	public class PoolController : MonoBehaviour 
	{
		public List<AutoCreatedPools> poolsToCreate;

		public static PoolController Instance;
		public bool debugMode = false;

		protected Dictionary<int, Pool> _poolDictionary;
		protected bool _isDirty = false;
		protected List<GameObject> _spawnedGameObjects;

		protected virtual void Awake() 
		{
			if(!Instance)
			{
				Instance = this;
			}
			else
			{
				Debug.LogError(string.Format("There is already a {0} in the scene.", GetType()));
			}
			
			_poolDictionary = new Dictionary<int, Pool>();
			_spawnedGameObjects = new List<GameObject>();

			foreach(AutoCreatedPools poolToCreate in poolsToCreate)
			{
				ObjectPooling.Pool.CreatePool(poolToCreate.prefab, poolToCreate.amount, poolToCreate.emptyBehavior, true);
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

		public virtual void CreatePool(GameObject prefab, int poolSize = 10, PoolEmptyBehavior poolEmptyBehavior = PoolEmptyBehavior.ExpandPool, bool overrideExisting = false)
		{
			int poolKey = prefab.GetInstanceID();

			if(!_poolDictionary.ContainsKey(poolKey) || overrideExisting)
			{
				if(debugMode) Debug.LogFormat("Creating object pool for {0} with InstanceID of {1}", prefab.name, poolKey);

				Queue<GameObject> oldQueue = new Queue<GameObject>();

				if(_poolDictionary.ContainsKey(poolKey)) 
				{
					oldQueue = _poolDictionary[poolKey].queue;
					_poolDictionary.Remove(poolKey);
				}

				_poolDictionary.Add(poolKey, new Pool(prefab, poolEmptyBehavior));
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
		}

		public virtual GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			int poolKey = prefab.GetInstanceID();

			if(_poolDictionary.ContainsKey(poolKey)) //Spawn object
			{
				if(_poolDictionary[poolKey].queue.Count <= 0) // If empty
				{
					switch(_poolDictionary[poolKey].PoolEmptyBehavior)
					{
						case(PoolEmptyBehavior.ExpandPool):
							ExpandPool(prefab);
							break;
						case(PoolEmptyBehavior.ReuseObject):
							break;
						case(PoolEmptyBehavior.DoNothing):
							return null;
					}
				}

				GameObject poolObject = _poolDictionary[poolKey].queue.Dequeue();
				if(_poolDictionary[poolKey].PoolEmptyBehavior == PoolEmptyBehavior.ReuseObject) _poolDictionary[poolKey].queue.Enqueue(poolObject);
				
				poolObject.transform.position = position; // Set object transforms
				poolObject.transform.rotation = rotation;
				poolObject.transform.SetParent(parent);
				poolObject.SetActive(true);

				_isDirty = true; // Add it to the list of objects that need OnSpawn called
				_spawnedGameObjects.Add(poolObject);

				return poolObject;
			}
			else //Create pool and retry
			{
				if(debugMode) Debug.LogFormat("Prefab {0} does not have a pool yet, creating now.", prefab.name);
				CreatePool(prefab);
				return Spawn(prefab, position, rotation, parent);
			}
		}

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

				if(_poolDictionary[poolKey].PoolEmptyBehavior != PoolEmptyBehavior.ReuseObject) _poolDictionary[poolKey].queue.Enqueue(gameObjectToDespawn); // If we haven't added it to the pool yet
			}
			else
			{
				Debug.LogWarningFormat("{0} does not have a corresponding pool and will be destroyed instead.", gameObjectToDespawn.name);
				Destroy(gameObjectToDespawn);
			}
		}

		public class Pool
		{
			public GameObject prefab;
			public Queue<GameObject> queue;
            private PoolEmptyBehavior poolEmptyBehavior;

            public Pool(GameObject _prefab, PoolEmptyBehavior behavior)
			{
				prefab = _prefab;
				queue = new Queue<GameObject>();
				PoolEmptyBehavior = behavior;
			}

            public PoolEmptyBehavior PoolEmptyBehavior
            {
                get
                {
                    return poolEmptyBehavior;
                }

                private set
                {
                    poolEmptyBehavior = value;
                }
            }
        }

		[System.Serializable]
		public class AutoCreatedPools
		{
			public GameObject prefab;
			public int amount;
			public PoolEmptyBehavior emptyBehavior= PoolEmptyBehavior.ExpandPool;
		}
	}
}
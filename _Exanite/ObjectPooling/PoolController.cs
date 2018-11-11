using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exanite.ObjectPooling;
using RotaryHeart.Lib.SerializableDictionary; // If no RotaryHeart, delete this line

namespace Exanite.ObjectPooling.Internal
{
	public class PoolController : MonoBehaviour 
	{
		public static PoolController Instance;
		public bool debugMode = false;

		[SerializeField]private PoolDictionary m_poolDictionary; // If no RotaryHeart, replace PoolDictionary with "Dictionary<int, Pool>"
		[System.Serializable]public class PoolDictionary : SerializableDictionaryBase<int, Pool> {} // If no RotaryHeart, delete this line

		private void Awake() 
		{
			if(!Instance)
			{
				Instance = this;
			}
			else
			{
				Debug.LogError(string.Format("There is already a {0} in the scene.", GetType()));
			}

			if(m_poolDictionary.Count >= 1)
			{
				Debug.LogWarningFormat("{0} does not support manually added object pools, please use {1} instead. Clear the dictionary to stop this error from showing", GetType(), typeof(PoolAutoCreate));
			}

			m_poolDictionary = new PoolDictionary(); // If no RotaryHeart, replace PoolDictionary with "Dictionary<int, Pool>"
		}

		public void CreatePool(GameObject prefab, int poolSize = 10, PoolEmptyBehavior poolEmptyBehavior = PoolEmptyBehavior.ExpandPool)
		{
			int poolKey = prefab.GetInstanceID();

			if(!m_poolDictionary.ContainsKey(poolKey))
			{
				if(debugMode) Debug.LogFormat("Creating object pool for {0} with InstanceID of {1}", prefab.name, poolKey);
				
				m_poolDictionary.Add(poolKey, new Pool(prefab, poolEmptyBehavior));

				ExpandPool(prefab, poolSize);
			}
		}

		public void ExpandPool(GameObject prefab, int amount = 5)
		{
			int poolKey = prefab.GetInstanceID();

			if(m_poolDictionary.ContainsKey(poolKey))
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

					m_poolDictionary[poolKey].queue.Enqueue(poolObject);
					m_poolDictionary[poolKey].poolSize++;
					m_poolDictionary[poolKey].objectsLeft++;
				}
			}
		}

		public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			int poolKey = prefab.GetInstanceID();

			if(m_poolDictionary.ContainsKey(poolKey)) //Spawn object
			{
				if(m_poolDictionary[poolKey].objectsLeft <= 0)
				{
					switch(m_poolDictionary[poolKey].poolEmptyBehavior)
					{
						case(PoolEmptyBehavior.ExpandPool):
							ExpandPool(prefab);
							break;
						case(PoolEmptyBehavior.ReuseObject):
							m_poolDictionary[poolKey].objectsLeft++;
							m_poolDictionary[poolKey].objectsSpawned--;
							break;
						case(PoolEmptyBehavior.DoNothing):
							return null;
					}
				}

				GameObject poolObject = m_poolDictionary[poolKey].queue.Dequeue();
				m_poolDictionary[poolKey].queue.Enqueue(poolObject);
				
				poolObject.transform.position = position;
				poolObject.transform.rotation = rotation;
				poolObject.transform.SetParent(parent);
				poolObject.SetActive(true);

				foreach(IPoolable iPoolable in poolObject.GetComponentsInChildren<IPoolable>())
				{
					iPoolable.OnSpawn();
				}

				m_poolDictionary[poolKey].objectsLeft--;
				m_poolDictionary[poolKey].objectsSpawned++;

				return poolObject;
			}
			else //Create pool and retry
			{
				if(debugMode) Debug.LogFormat("Prefab {0} does not have a pool yet, creating now.", prefab.name);
				CreatePool(prefab);
				return Spawn(prefab, position, rotation, parent);
			}
		}

		public void Despawn(GameObject gameObjectToDespawn)
		{
			int poolKey = gameObjectToDespawn.GetComponent<PoolInstanceID>().instanceID;

			if(m_poolDictionary.ContainsKey(poolKey))
			{
				foreach(IPoolable iPoolable in gameObjectToDespawn.GetComponentsInChildren<IPoolable>())
				{
					iPoolable.OnDespawn();
				}

				gameObjectToDespawn.SetActive(false);
				gameObjectToDespawn.transform.SetParent(transform);

				m_poolDictionary[poolKey].objectsLeft++;
				m_poolDictionary[poolKey].objectsSpawned--;
			}
			else
			{
				Debug.LogWarningFormat("{0} does not have a corresponding pool and will not be despawned.", gameObjectToDespawn.name);
			}
		}

		[System.Serializable]
		public class Pool
		{
			public GameObject prefab;
			public Queue<GameObject> queue;
			public PoolEmptyBehavior poolEmptyBehavior;

			public int objectsSpawned = 0;
			public int objectsLeft = 0;
			public int poolSize = 0;

			public Pool(GameObject _prefab, PoolEmptyBehavior behavior)
			{
				prefab = _prefab;
				queue = new Queue<GameObject>();
				poolEmptyBehavior = behavior;
			}
		}
	}
}
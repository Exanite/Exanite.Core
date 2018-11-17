using UnityEngine;
using Exanite.ObjectPooling.Internal;

namespace Exanite.ObjectPooling
{
    public static class Pool
	{
		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			return PoolController.Instance.Spawn(prefab, position, rotation, parent);
		}

		public static void Despawn(GameObject gameObjectToDespawn)
		{
			PoolController.Instance.Despawn(gameObjectToDespawn);
		}
		
		public static void CreatePool(GameObject prefab, int poolSize = 1, PoolEmptyBehavior poolEmptyBehavior = PoolEmptyBehavior.ExpandPool, bool overrideExisting = false)
		{
			PoolController.Instance.CreatePool(prefab, poolSize, poolEmptyBehavior, overrideExisting);
		}

		public static void ExpandPool(GameObject prefab, int amount = 1)
		{
			PoolController.Instance.ExpandPool(prefab, amount);
		}
	}
}

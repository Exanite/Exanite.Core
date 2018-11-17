using UnityEngine;

namespace Exanite.ObjectPooling.Example
{
    public class PoolUseExample : MonoBehaviour 
	{
		public GameObject prefab;

		private void Start() 
		{
			Pool.CreatePool(prefab, 23, PoolEmptyBehavior.ExpandPool); // Not needed, create pool manually with 23 pooled objects

			Pool.ExpandPool(prefab, 15); // Manually expand previous pool by 15

			GameObject example = Pool.Spawn(prefab, transform.position, transform.rotation, transform); // Spawning a prefab from the pool

			Pool.Despawn(example); // Despawning the previously spawned GameObject
		}
	}
}
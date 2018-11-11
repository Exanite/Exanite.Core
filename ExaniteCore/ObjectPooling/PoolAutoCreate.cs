using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.ObjectPooling.Internal
{
	public class PoolAutoCreate : MonoBehaviour 
	{
		public List<PoolToCreate> prefabs;

		private void Start() 
		{
			foreach(PoolToCreate poolToCreate in prefabs)
			{
				Pool.CreatePool(poolToCreate.prefab, poolToCreate.amount, poolToCreate.emptyBehavior);
			}
		}

		[System.Serializable]
		public class PoolToCreate
		{
			public GameObject prefab;
			public int amount;
			public PoolEmptyBehavior emptyBehavior= PoolEmptyBehavior.ExpandPool;
		}
	}
}
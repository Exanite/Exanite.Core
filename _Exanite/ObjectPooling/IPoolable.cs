using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.ObjectPooling
{
	public interface IPoolable
	{
		void OnSpawn();

		void OnDespawn(); // Use this for reseting the GameObject's variables
	}
}
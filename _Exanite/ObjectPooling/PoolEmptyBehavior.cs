using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.ObjectPooling
{
	public enum PoolEmptyBehavior
	{
		ExpandPool = 0,
		ReuseObject = 1,
		DoNothing = 2,
	}
}
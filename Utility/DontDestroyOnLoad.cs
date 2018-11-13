using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.Utility
{
	public class DontDestroyOnLoad : MonoBehaviour 
	{
		protected virtual void Awake() 
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
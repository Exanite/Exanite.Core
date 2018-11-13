using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	public class BTBlackboard // Use this to save variables to the tree
	{
		protected Dictionary<string, object> blackboard;

		public BTBlackboard(params KeyValuePair<string, object>[] vars)
		{
			blackboard = new Dictionary<string, object>();

			foreach (KeyValuePair<string, object> var in vars)
			{
				blackboard.Add(var.Key, var.Value);
			}
		}

		public object GetVariable(string key) 
		{

            if(Exists(key)) return blackboard[key];
			// else
			throw new KeyNotFoundException(string.Format("The key {0} does not exist in {1}", key, blackboard));
        }

        public void SetVariable(string key, object value) 
		{
            blackboard[key] = value;
        }

        public bool Exists(string key) 
		{
            return blackboard.ContainsKey(key);
        }
	}
}
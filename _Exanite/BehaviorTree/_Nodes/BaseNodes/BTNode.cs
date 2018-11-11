using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Base class for all Nodes
	public abstract class BTNode 
	{
		protected bool started = false;
		protected BTState nodeState = BTState.NotStarted;

		protected BTBlackboard blackboard;

		// Use this to process another tick in the tree
		public virtual void ProcessTick(ref BTBlackboard blackboard) 
		{
			if(!started) 
			{
				started = true;
				nodeState = BTState.Running;
				Start();
			}

			Update();

			if(nodeState != BTState.Running) End();
		}

		// Start coroutines here/etc, things that should only be run once
		protected virtual void Start() { }

		// Updates the node, similar to MonoBehavior's Update()
		protected virtual void Update() { }

		// Resets the node for later use
		public virtual void End() 
		{
			if(started)
			{
				started = false;
			}
		}

		public virtual BTState GetState()
		{
			return nodeState;
		}
	}
}
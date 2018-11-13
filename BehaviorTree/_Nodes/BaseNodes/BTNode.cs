using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Base class for all Nodes
	public abstract class BTNode 
	{
		protected bool _started = false;
		protected BTState _nodeState = BTState.NotStarted;

		protected BTBlackboard _blackboard;

		// Use this to process another tick in the tree
		public virtual void ProcessTick(ref BTBlackboard blackboard) 
		{
			if(!_started) 
			{
				_started = true;
				_nodeState = BTState.Running;
				Start();
			}

			Update();

			if(_nodeState != BTState.Running) End();
		}

		// Start coroutines here/etc, things that should only be run once
		protected virtual void Start() { }

		// Updates the node, similar to MonoBehavior's Update()
		protected virtual void Update() { }

		// Resets the node for later use
		public virtual void End() 
		{
			if(_started)
			{
				_started = false;
			}
		}

		public virtual BTState GetState()
		{
			return _nodeState;
		}
	}
}
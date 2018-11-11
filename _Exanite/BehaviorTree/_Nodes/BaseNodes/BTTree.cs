using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Root of the behavior tree, define a new tree with this
	public class BTTree : BTNodeComposite 
	{
		public BTTree(params BTNode[] nodes) : base(nodes)
		{
			blackboard = new BTBlackboard();
		}

		// Use this to use the internal blackboard, or use ProcessTick(Blackboard) to supply your own
		public virtual void ProcessTick() 
		{
			ProcessTick(ref blackboard);
		}

		protected override void ChildQueueReset()
		{
			childQueue = new Queue<BTNode>(childNodes);
		}

		protected override void Start()
		{
			ChildQueueReset();

			StartChildren();

			nodeState = BTState.Succeeded;
		}

		protected override void StartChildren()
		{
			for (int i = 0; i < childQueue.Count; i++)
			{
				BTNode node = ChildQueueDequeue();

				StartChild(node);
			}
		}
		
		public override void End() 
		{
			if(started)
			{
				started = false;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Composite node - If any fail, stop and fail node, if all succeed, return succeeded
	public class BTSequence : BTNodeComposite
	{
		protected BTNode currentNode;

		public BTSequence(params BTNode[] nodes) : base(nodes) { }
		
		protected override void Start()
		{
			ChildQueueReset();
		}
		
		protected override void Update()
		{
			StartChildren();
		}
		
		protected override void ChildQueueReset()
		{
			childQueue = new Queue<BTNode>(childNodes);
		}
		
		protected override void StartChildren()
		{
			if(currentNode == null || currentNode.GetState() != BTState.Running)
			{
				// Find first node that fails or is still running
				int timesToRun = childQueue.Count;
				for(int i = 0; i < timesToRun; i++)
				{
					BTNode tempNode = ChildQueueDequeue();

					StartChild(tempNode);

					switch(tempNode.GetState())
					{
						case(BTState.Failed):
							nodeState = BTState.Failed;
							EndChildren();
							return;
						case(BTState.Running):
							currentNode = tempNode;
							return;
					}
				}
			}
			else // Run current node until it is no longer running
			{
				StartChild(currentNode);

				switch(currentNode.GetState())
				{
					case(BTState.Failed):
						nodeState = BTState.Failed;
						EndChildren();
						return;
					case(BTState.Running):
						return;
					case(BTState.Succeeded):
						currentNode = null;
						break;
				}
			}

			// If all children succeeded 
			// If the node succeeds when ran above, this will test if it is the last in the sequence
			if(currentNode == null && childQueue.Count <= 0) 
			{
				nodeState = BTState.Succeeded;
				return;
			}
		}
	}
}
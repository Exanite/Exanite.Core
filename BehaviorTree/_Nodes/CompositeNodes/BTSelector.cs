using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Exanite.BehaviorTree
{
	// Composite node - If any child succeeds stop and return succeeded
	public class BTSelector : BTNodeComposite
	{
		protected BTNode _currentNode;

		public BTSelector(params BTNode[] nodes) : base(nodes) { }

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
			List<BTNode> nodesToAdd = childNodes.OrderBy(x => Random.value).ToList();

			childQueue = new Queue<BTNode>(nodesToAdd);
		}

		protected override void StartChildren()
		{
			if(_currentNode == null || _currentNode.GetState() != BTState.Running)
			{
				// Find first node that returns succeeded or running
				for (int i = 0; i < childQueue.Count; i++)
				{
					BTNode tempNode = ChildQueueDequeue();

					StartChild(tempNode);

					switch(tempNode.GetState())
					{
						case(BTState.Succeeded):
							_nodeState = BTState.Succeeded;
							return;
						case(BTState.Running):
							_currentNode = tempNode;
							_nodeState = BTState.Running;
							return;
					}
				}
			}
			else // Run current node until it is no longer running
			{
				StartChild(_currentNode);

				switch(_currentNode.GetState())
				{
					case(BTState.Succeeded):
						_nodeState = BTState.Succeeded;
						return;
					case(BTState.Running):
						_nodeState = BTState.Running;
						return;
				}
			}

			// If all children failed
			// If the node fails when ran above, this will test if it is the last in the sequence
			if(_currentNode == null && childQueue.Count <= 0) 
			{
				_nodeState = BTState.Failed;
				return;
			}
		}
	}
}
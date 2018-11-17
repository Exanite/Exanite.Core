using System.Collections.Generic;

namespace Exanite.BehaviorTree
{
    // Base class for all nodes that will have a child
    public abstract class BTNodeParent : BTNode
	{
		public List<BTNode> childNodes;

		// Takes in an array of nodes as child nodes
		public BTNodeParent(params BTNode[] nodes) 
		{
			childNodes = new List<BTNode>();
			childNodes.AddRange(nodes);
		}

		protected virtual void StartChild(BTNode node)
		{
			node.ProcessTick(ref _blackboard);
		}

		protected virtual void EndChild(BTNode node)
		{
			node.End();
		}

		// Starts child nodes and handles the return state
		protected virtual void EndChildren() 
		{
			foreach(BTNode node in childNodes)
			{
				node.End();
			}
		}

		public override void End()
		{
			EndChildren();
			_started = false;
		}
	}
}
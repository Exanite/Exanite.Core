using System.Collections.Generic;

namespace Exanite.BehaviorTree
{
    // Base class for all nodes that can support more than one child
    public abstract class BTNodeComposite : BTNodeParent 
	{
		protected Queue<BTNode> childQueue;

		public BTNodeComposite(params BTNode[] nodes) : base(nodes) {}

		// Starts child nodes and handles the return state
		protected abstract void StartChildren();

		protected virtual BTNode ChildQueueDequeue()
		{
			return childQueue.Dequeue();
		}

		protected virtual void ChildQueueEnqueue(BTNode node)
		{
			childQueue.Enqueue(node);
		}

		protected abstract void ChildQueueReset();
	}
}
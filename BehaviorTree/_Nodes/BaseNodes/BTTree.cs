using System.Collections.Generic;

namespace Exanite.BehaviorTree
{
    // Root of the behavior tree, define a new tree with this
    public class BTTree : BTNodeComposite 
    {
        public BTTree(params BTNode[] nodes) : base(nodes)
        {
            _blackboard = new BTBlackboard();
        }

        // Use this to use the internal blackboard, or use ProcessTick(Blackboard) to supply your own
        public virtual void ProcessTick() 
        {
            ProcessTick(ref _blackboard);
        }

        protected override void ChildQueueReset()
        {
            childQueue = new Queue<BTNode>(childNodes);
        }

        protected override void Start()
        {
            ChildQueueReset();

            StartChildren();

            _nodeState = BTState.Succeeded;
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
            if(_started)
            {
                _started = false;
            }
        }
    }
}
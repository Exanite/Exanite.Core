using System.Collections.Generic;

namespace Exanite.BehaviorTree
{
    // Composite node - If any fail, stop and fail node, if all succeed, return succeeded
    public class BTSequence : BTNodeComposite
    {
        protected BTNode _currentNode;

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
            if(_currentNode == null || _currentNode.GetState() != BTState.Running)
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
                            _nodeState = BTState.Failed;
                            EndChildren();
                            return;
                        case(BTState.Running):
                            _currentNode = tempNode;
                            return;
                    }
                }
            }
            else // Run current node until it is no longer running
            {
                StartChild(_currentNode);

                switch(_currentNode.GetState())
                {
                    case(BTState.Failed):
                        _nodeState = BTState.Failed;
                        EndChildren();
                        return;
                    case(BTState.Running):
                        return;
                    case(BTState.Succeeded):
                        _currentNode = null;
                        break;
                }
            }

            // If all children succeeded 
            // If the node succeeds when ran above, this will test if it is the last in the sequence
            if(_currentNode == null && childQueue.Count <= 0) 
            {
                _nodeState = BTState.Succeeded;
                return;
            }
        }
    }
}
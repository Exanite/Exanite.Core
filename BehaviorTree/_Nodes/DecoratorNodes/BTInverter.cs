namespace Exanite.BehaviorTree
{
    // Decorator node - Inverts result of child
    public class BTInverter : BTNodeDecorator 
    {
        public BTInverter(BTNode node) : base(node) { }

        protected override void Update()
        {
            StartChild(childNodes[0]);

            switch(childNodes[0].GetState())
            {
                case(BTState.Running):
                    break;
                case(BTState.Failed):
                    _nodeState = BTState.Succeeded;
                    break;
                case(BTState.Succeeded):
                    _nodeState = BTState.Failed;
                    break;
            }
        }
    }
}
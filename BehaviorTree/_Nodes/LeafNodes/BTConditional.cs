using System;

namespace Exanite.BehaviorTree
{
    // Takes in a Func<bool> and returns succeeded(true) or failed(false)
    public class BTConditional : BTNodeLeaf 
    {
        public Func<bool> Expression;

        public BTConditional(Func<bool> expression)
        {
            this.Expression = expression;
        }

        protected override void Update()
        {
            switch(EvaluateExpression())
            {
                case(true):
                    _nodeState = BTState.Succeeded;
                    break;
                case(false):
                    _nodeState = BTState.Failed;
                    break;
            }
        }

        protected virtual bool EvaluateExpression()
        {
            return Expression();
        }
    }
}
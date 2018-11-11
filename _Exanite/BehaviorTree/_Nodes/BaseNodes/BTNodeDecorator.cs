using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Base class for all nodes that can only have one child
	public abstract class BTNodeDecorator : BTNodeParent
	{
		public BTNodeDecorator(BTNode node) : base(node) {}
	}
}
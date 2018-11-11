using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exanite.BehaviorTree;
using System;

public class BehaviorTreeTest : MonoBehaviour 
{
	public int a = 15;
	public int b = 20;
	public int c = 10;

	public bool runCoroutine = false;

	public BTTree tree;

	private void Start() 
	{
		Func<bool> aGreaterThanB = (() => a > b); // false
		Func<bool> aGreaterThanC = (() => a > c); // true
		Func<bool> aLessThanB = (() => a < b); // true
		Func<bool> shouldRun = (() => runCoroutine);

		tree = new BTTree(
					new BTSequence(
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTConditional(shouldRun),
						new BTCoroutine(Test(), this)));

		//tree.ProcessTick();
		//tree.ProcessTick();
	}

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.LogWarning("Processing another tick!");
			tree.ProcessTick();
		}
		//tree.ProcessTick();
	}

	IEnumerable Test()
	{
		Debug.LogFormat("{0} Test() started!", this);
		yield return new WaitForSeconds(2f);
		Debug.LogFormat("{0}'s Test() middle!", this);
		yield return new WaitForSeconds(2f);
		Debug.LogFormat("{0}'s Test() ended!", this);
	}
}
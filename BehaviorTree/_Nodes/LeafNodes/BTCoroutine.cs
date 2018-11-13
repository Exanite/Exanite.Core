using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.BehaviorTree
{
	// Leaf node - This runs a coroutine and returns succeeded on finish
	public class BTCoroutine : BTNodeLeaf
	{
		protected IEnumerable m_iEnumerable;
		protected MonoBehaviour m_monoBehaviour;

		protected Coroutine _currentCoroutine;
		protected bool _running = false;

		public BTCoroutine(IEnumerable m_iEnumerable, MonoBehaviour m_monoBehaviour)
		{
			this.m_iEnumerable = m_iEnumerable;
			this.m_monoBehaviour = m_monoBehaviour;
		}

		protected override void Start()
		{
			m_monoBehaviour.StartCoroutine(CoroutineWrapper(m_iEnumerable));
		}

		protected override void Update()
		{
			if(!_running)
			{
				nodeState = BTState.Succeeded;
			}
		}

		public override void End()
		{
			started = false;
			if(_currentCoroutine != null) {m_monoBehaviour.StopCoroutine(_currentCoroutine);}
			_currentCoroutine = null;
		}

		IEnumerator CoroutineWrapper(IEnumerable iEnumerable)
		{
			_running = true;
			yield return _currentCoroutine = m_monoBehaviour.StartCoroutine(iEnumerable.GetEnumerator());
			m_monoBehaviour.StopCoroutine(_currentCoroutine);
			_currentCoroutine = null;
			_running = false;
		}
	}
}
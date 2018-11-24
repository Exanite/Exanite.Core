using System.Collections;
using UnityEngine;

namespace Exanite.BehaviorTree
{
    // Leaf node - This runs a coroutine and returns succeeded on finish
    public class BTCoroutine : BTNodeLeaf
	{
		protected IEnumerable _iEnumerable;
		protected MonoBehaviour _monoBehaviour;

		protected Coroutine _currentCoroutine;
		protected bool _running = false;

		public BTCoroutine(IEnumerable m_iEnumerable, MonoBehaviour m_monoBehaviour)
		{
			this._iEnumerable = m_iEnumerable;
			this._monoBehaviour = m_monoBehaviour;
		}

		protected override void Start()
		{
			_monoBehaviour.StartCoroutine(CoroutineWrapper(_iEnumerable));
		}

		protected override void Update()
		{
			if(!_running)
			{
				_nodeState = BTState.Succeeded;
			}
		}

		public override void End()
		{
			_started = false;
			if(_currentCoroutine != null) {_monoBehaviour.StopCoroutine(_currentCoroutine);}
			_currentCoroutine = null;
		}

		protected virtual IEnumerator CoroutineWrapper(IEnumerable iEnumerable)
		{
			_running = true;
			yield return _currentCoroutine = _monoBehaviour.StartCoroutine(iEnumerable.GetEnumerator());
			_monoBehaviour.StopCoroutine(_currentCoroutine);
			_currentCoroutine = null;
			_running = false;
		}
	}
}
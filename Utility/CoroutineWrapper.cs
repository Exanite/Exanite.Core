using System;
using System.Collections;
using UnityEngine;

namespace Exanite.Utility
{
    public class CoroutineWrapper
    {
        private MonoBehaviour monoBehaviour;

        public Coroutine coroutine;
        public bool running = false;
        public Action CoroutineFinished;

        public CoroutineWrapper(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            monoBehaviour.StartCoroutine(Wrapper(routine));
            return coroutine;
        }

        public IEnumerator Wrapper(IEnumerator routine)
        {
            running = true;
            yield return coroutine = monoBehaviour.StartCoroutine(routine);

            CoroutineFinished?.Invoke();
            running = false;
            coroutine = null;
        }
    }
}

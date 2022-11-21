using System;
using UnityEngine;

namespace Exanite.Core.Components
{
    [DefaultExecutionOrder(-100)]
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T instance;

        public bool dontDestroyOnLoad;

        private bool hasInitialized;

        public static T Instance
        {
            get
            {
                if (instance)
                {
                    return instance;
                }

                throw new NullReferenceException($"{typeof(T).Name} was null. There should be a {typeof(T).Name} object somewhere in your scene.");
            }
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (hasInitialized)
            {
                return;
            }

            hasInitialized = true;

            if (instance)
            {
                Destroy(gameObject);

                return;
            }

            instance = (T)this;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }
        }
    }
}
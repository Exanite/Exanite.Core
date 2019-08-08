using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components
{
    /// <summary>
    /// Reusable singleton implementation for SerializedMonoBehaviours
    /// </summary>
    public abstract class SingletonSerializedBehaviour<T> : SerializedMonoBehaviour where T : SingletonSerializedBehaviour<T>
    {
        private static T instance;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    new GameObject(typeof(T).ToString(), typeof(T));
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }
    }
}

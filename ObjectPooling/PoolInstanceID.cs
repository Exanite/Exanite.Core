using UnityEngine;

namespace Exanite.ObjectPooling
{
    /// <summary>
    /// Used by ObjectPooling to store prefab data
    /// </summary>
    public class PoolInstanceID : MonoBehaviour 
    {
        /// <summary>
        /// Original version of this GameObject, can be used to copy values
        /// </summary>
        public GameObject originalPrefab;
        /// <summary>
        /// InstanceID of the original prefab
        /// </summary>
        public int instanceID;
    }
}
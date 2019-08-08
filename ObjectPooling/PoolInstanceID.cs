using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Used by ObjectPooling to store prefab data
    /// </summary>
    public class PoolInstanceID : SerializedMonoBehaviour
    {
        /// <summary>
        /// Original version of this GameObject, can be used to copy values
        /// </summary>
        [OdinSerialize] public GameObject OriginalPrefab { get; set; }

        /// <summary>
        /// InstanceID of the original prefab
        /// </summary>
        [OdinSerialize] public int InstanceID { get; set; }
    }
}
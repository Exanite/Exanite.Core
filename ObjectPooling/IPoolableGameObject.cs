using UnityEngine;

namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Interface used to call methods when a <see cref="GameObject"/> is spawned from or despawned to an <see cref="GameObjectPool"/>
    /// </summary>
    public interface IPoolableGameObject
    {
        /// <summary>
        /// Called when a GameObject is spawned
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called when a GameObject is despawned
        /// </summary>
        void OnDespawn();
    }
}
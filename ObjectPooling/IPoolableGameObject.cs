﻿namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Interface used in the object pool to call methods on GameObject spawn and despawn
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
namespace Exanite.Core.ObjectPooling
{
    /// <summary>
    /// Interface used to call methods when an object is acquired or released from an <see cref="ObjectPool{T}"/>
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when an object is acquired
        /// </summary>
        void OnAcquired();

        /// <summary>
        /// Called when an object is released
        /// </summary>
        void OnReleased();
    }
}

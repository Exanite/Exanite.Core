using UnityEngine;

namespace Exanite.Core.ObjectPooling.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class PooledRigidbody : MonoBehaviour, IPoolableGameObject
    {
        private Rigidbody _rigidbody;

        private PoolInstanceID instanceID;
        private Rigidbody originalRigidbody;

        private void Start()
        {
            instanceID = GetComponent<PoolInstanceID>();
            if (!instanceID)
            {
                Destroy(this);
            }

            _rigidbody = GetComponent<Rigidbody>();
            originalRigidbody = instanceID.OriginalPrefab.GetComponent<Rigidbody>();
        }

        public void OnSpawn()
        {
            _rigidbody.WakeUp();
        }

        public void OnDespawn()
        {
            _rigidbody.Sleep();

            _rigidbody.mass             = originalRigidbody.mass;

            _rigidbody.angularDrag      = originalRigidbody.angularDrag;
            _rigidbody.angularVelocity  = originalRigidbody.angularVelocity;
            _rigidbody.drag             = originalRigidbody.drag;
            _rigidbody.velocity         = originalRigidbody.velocity;
            
            _rigidbody.constraints      = originalRigidbody.constraints;
            _rigidbody.isKinematic      = originalRigidbody.isKinematic;
            _rigidbody.useGravity       = originalRigidbody.useGravity;

            _rigidbody.centerOfMass             = originalRigidbody.centerOfMass;
            _rigidbody.collisionDetectionMode   = originalRigidbody.collisionDetectionMode;
            _rigidbody.detectCollisions         = originalRigidbody.detectCollisions;
            _rigidbody.inertiaTensor            = originalRigidbody.inertiaTensor;
            _rigidbody.inertiaTensorRotation    = originalRigidbody.inertiaTensorRotation;
            _rigidbody.interpolation            = originalRigidbody.interpolation;
            _rigidbody.maxAngularVelocity       = originalRigidbody.maxAngularVelocity;
            _rigidbody.maxDepenetrationVelocity = originalRigidbody.maxDepenetrationVelocity;
            _rigidbody.sleepThreshold           = originalRigidbody.sleepThreshold;
            _rigidbody.solverIterations         = originalRigidbody.solverIterations;
            _rigidbody.solverVelocityIterations = originalRigidbody.solverVelocityIterations;
        }
    }
}
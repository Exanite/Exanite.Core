using UnityEngine;

namespace Exanite.ObjectPooling.Utility
{
    [RequireComponent(typeof(Rigidbody))]
    public class PooledRigidbody : MonoBehaviour, IPoolable
    {
        private Rigidbody _rigidbody;

        private PoolInstanceID _instanceID;
        private Rigidbody _originalRigidbody;

        private void Start()
        {
            _instanceID = GetComponent<PoolInstanceID>();
            if (!_instanceID) Destroy(this);

            _rigidbody = GetComponent<Rigidbody>();
            _originalRigidbody = _instanceID.originalPrefab.GetComponent<Rigidbody>();
        }

        public void OnSpawn()
        {
            _rigidbody.WakeUp();
        }

        public void OnDespawn()
        {
            _rigidbody.Sleep();

            _rigidbody.mass             = _originalRigidbody.mass;

            _rigidbody.angularDrag      = _originalRigidbody.angularDrag;
            _rigidbody.angularVelocity  = _originalRigidbody.angularVelocity;
            _rigidbody.drag             = _originalRigidbody.drag;
            _rigidbody.velocity         = _originalRigidbody.velocity;
            
            _rigidbody.constraints      = _originalRigidbody.constraints;
            _rigidbody.isKinematic      = _originalRigidbody.isKinematic;
            _rigidbody.useGravity       = _originalRigidbody.useGravity;

            _rigidbody.centerOfMass             = _originalRigidbody.centerOfMass;
            _rigidbody.collisionDetectionMode   = _originalRigidbody.collisionDetectionMode;
            _rigidbody.detectCollisions         = _originalRigidbody.detectCollisions;
            _rigidbody.inertiaTensor            = _originalRigidbody.inertiaTensor;
            _rigidbody.inertiaTensorRotation    = _originalRigidbody.inertiaTensorRotation;
            _rigidbody.interpolation            = _originalRigidbody.interpolation;
            _rigidbody.maxAngularVelocity       = _originalRigidbody.maxAngularVelocity;
            _rigidbody.maxDepenetrationVelocity = _originalRigidbody.maxDepenetrationVelocity;
            _rigidbody.sleepThreshold           = _originalRigidbody.sleepThreshold;
            _rigidbody.solverIterations         = _originalRigidbody.solverIterations;
            _rigidbody.solverVelocityIterations = _originalRigidbody.solverVelocityIterations;
        }
    }
}
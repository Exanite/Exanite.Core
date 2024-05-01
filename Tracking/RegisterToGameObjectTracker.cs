using UniDi;
using UnityEngine;

namespace Exanite.Core.Tracking
{
    public class RegisterToGameObjectTracker : MonoBehaviour
    {
        [Inject] private GameObjectTracker tracker;

        private void OnEnable()
        {
            tracker.Register(gameObject);
        }

        private void OnDisable()
        {
            tracker.Unregister(gameObject);
        }
    }
}

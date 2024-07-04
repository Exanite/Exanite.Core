#if EXANITE_UNIDI
using UniDi;
using UnityEngine;

namespace Exanite.Core.Tracking
{
    public class RegisterToGameObjectTracker : MonoBehaviour
    {
        [Inject] private GameObjectTracker tracker = null!;

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
#endif

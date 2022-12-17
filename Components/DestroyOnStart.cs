using UnityEngine;

namespace Exanite.Core.Components
{
    public class DestroyOnStart : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject);
        }
    }
}

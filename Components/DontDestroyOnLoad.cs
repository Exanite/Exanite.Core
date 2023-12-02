using UnityEngine;

namespace Exanite.Core.Components
{
    /// <summary>
    /// Marks the GameObject this component is attached to as
    /// DontDestroyOnLoad on Awake
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

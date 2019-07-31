using UnityEngine;

namespace Exanite.Core.Components
{
    public class DontDestroyOnLoad : MonoBehaviour 
    {
        private void Awake() 
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
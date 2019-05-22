using UnityEngine;

namespace Exanite.Core.Utility
{
    public class DontDestroyOnLoad : MonoBehaviour 
    {
        private void Awake() 
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
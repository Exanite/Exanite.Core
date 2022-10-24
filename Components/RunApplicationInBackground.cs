using UnityEngine;

namespace Exanite.Core.Components
{
    public class RunApplicationInBackground : MonoBehaviour
    {
        private void Start()
        {
            Application.runInBackground = true;
        }
    }
}
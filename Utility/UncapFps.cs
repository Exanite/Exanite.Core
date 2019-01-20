using UnityEngine;

namespace Exanite.Utility
{
    public class UncapFps : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = -1;
        }
    }
}
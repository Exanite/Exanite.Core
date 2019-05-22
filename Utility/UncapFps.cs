using UnityEngine;

namespace Exanite.Core.Utility
{
    public class UncapFps : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = -1;
        }
    }
}
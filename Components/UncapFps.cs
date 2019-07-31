using UnityEngine;

namespace Exanite.Core.Components
{
    public class UncapFps : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = -1;
        }
    }
}
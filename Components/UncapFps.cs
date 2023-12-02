using UnityEngine;

namespace Exanite.Core.Components
{
    /// <summary>
    /// Sets the target frame rate to -1 (uncapped) on Start
    /// </summary>
    public class UncapFps : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = -1;
        }
    }
}

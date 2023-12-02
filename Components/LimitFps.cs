using UnityEngine;

namespace Exanite.Core.Components
{
    /// <summary>
    /// Sets the target frame rate to the specified TargetFps on Start.
    /// Can be optionally set to run only when
    /// <see cref="Application.isBatchMode"/> is true is used.
    /// </summary>
    public class LimitFps : MonoBehaviour
    {
        public bool IsBatchModeOnly;
        public int TargetFps = 30;

        private void Start()
        {
            if (Application.isBatchMode || !IsBatchModeOnly)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = TargetFps;
            }
        }
    }
}

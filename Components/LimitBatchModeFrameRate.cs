using UnityEngine;

namespace Exanite.Core.Components
{
    /// <summary>
    /// Used to limit the <see cref="Application.targetFrameRate"/> when the game is running in -batchmode
    /// </summary>
    public class LimitBatchModeFrameRate : MonoBehaviour
    {
        public int targetFrameRate = 30;
        
        private void Start()
        {
            if (Application.isBatchMode)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFrameRate;
            }
        }
    } 
}

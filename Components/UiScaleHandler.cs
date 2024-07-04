#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Exanite.Core.Components
{
    public class UiScaleHandler : MonoBehaviour
    {
        private static readonly string ScaleName = "Project_UiScale";

#if ODIN_INSPECTOR
        [Required]
#endif
        public CanvasScaler Scaler = null!;

        private void Start()
        {
            Scaler.scaleFactor = PlayerPrefs.HasKey(ScaleName) ? PlayerPrefs.GetInt(ScaleName) : 1;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
            {
                PlayerPrefs.SetInt(ScaleName, 2);
                Scaler.scaleFactor = 2;
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
            {
                PlayerPrefs.SetInt(ScaleName, 1);
                Scaler.scaleFactor = 1;
            }
        }
    }
}

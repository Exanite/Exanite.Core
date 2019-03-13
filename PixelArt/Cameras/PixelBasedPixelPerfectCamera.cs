using Exanite.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt.Cameras
{
    public class PixelBasedPixelPerfectCamera : PixelPerfectCameraBase
    {
        [SerializeField, HideInInspector]
        private int pixelsPerActualPixel = 10;

        [ShowInInspector]
        public int PixelsPerActualPixel
        {
            get
            {
                return pixelsPerActualPixel;
            }

            set
            {
                if (value != 0)
                {
                    pixelsPerActualPixel = value;
                }

                CalculateCameraSize();
            }
        }

        public override void CalculateCameraSize()
        {
            float verticalPixels = Screen.height / PixelsPerActualPixel;

            _camera.orthographicSize = verticalPixels / (Ppu * 2);
        }
    }
}

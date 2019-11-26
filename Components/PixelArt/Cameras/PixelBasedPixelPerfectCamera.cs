using Exanite.Core.Components.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras
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
            if (!Camera)
            {
                return;
            }

            float verticalPixels = CameraDimensions.y / PixelsPerActualPixel;

            Camera.orthographicSize = verticalPixels / (PixelsPerUnit * 2);
        }
    }
}
using Exanite.Core.Components.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras
{
    /// <summary>
    /// Pixel perfect camera that displays each pixel as a 'larger pixel' on screen
    /// </summary>
    public class PixelBasedPixelPerfectCamera : PixelPerfectCameraBase
    {
        [SerializeField, HideInInspector]
        private int pixelsPerActualPixel = 10;

        /// <summary>
        /// Pixels per actual pixel to display
        /// </summary>
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

        /// <summary>
        /// Calculates and sets the targeted <see cref="UnityEngine.Camera"/>'s orthographic size
        /// </summary>
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
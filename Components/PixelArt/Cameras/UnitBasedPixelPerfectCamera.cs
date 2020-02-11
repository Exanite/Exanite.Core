using System;
using Exanite.Core.Components.PixelArt.Cameras.Internal;
using Exanite.Core.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras
{
    /// <summary>
    /// Pixel perfect camera that keeps a certain amount of vertical units on screen
    /// </summary>
    public class UnitBasedPixelPerfectCamera : PixelPerfectCameraBase
    {
        [SerializeField, HideInInspector]
        private int verticalUnits = 10;

        /// <summary>
        /// Targeted amount of vertical units to display
        /// </summary>
        [ShowInInspector]
        public int VerticalUnits
        {
            get
            {
                return verticalUnits;
            }

            set
            {
                if (value != 0)
                {
                    verticalUnits = value;
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

            bool isNegative = VerticalUnits < 0;

            int unitSize = MathUtility.GetNearestMultiple(CameraDimensions.y / Math.Abs(VerticalUnits), PixelsPerUnit);

            if (unitSize <= 0 || unitSize == int.MaxValue)
            {
                unitSize = PixelsPerUnit;
            }

            Camera.orthographicSize = (isNegative ? -1 : 1) * CameraDimensions.y / (unitSize * 2f);
        }
    }
}
using System;
using Exanite.Core.Extensions;
using Exanite.Core.Components.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras
{
    public class UnitBasedPixelPerfectCamera : PixelPerfectCameraBase
    {
        [SerializeField, HideInInspector]
        private int verticalUnits = 10;

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

        public override void CalculateCameraSize()
        {
            if (!Camera)
            {
                return;
            }

            bool isNegative = VerticalUnits < 0;

            int unitSize = MathHelper.GetNearestMultiple(CameraDimensions.y / Math.Abs(VerticalUnits), PixelsPerUnit);

            if (unitSize <= 0 || unitSize == int.MaxValue)
            {
                unitSize = PixelsPerUnit;
            }

            Camera.orthographicSize = (isNegative ? -1 : 1) * CameraDimensions.y / (unitSize * 2f);
        }
    }
}
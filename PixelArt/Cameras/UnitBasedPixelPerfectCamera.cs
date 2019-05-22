using Exanite.Core.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Exanite.Core.PixelArt.Cameras
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
            if (!_camera)
            {
                return;
            }

            bool isNegative = VerticalUnits < 0;

            int unitSize = MathE.GetNearestMultiple(CameraDimensions.y / Math.Abs(VerticalUnits), Ppu);

            if (unitSize <= 0 || unitSize == int.MaxValue)
            {
                unitSize = Ppu;
            }

            _camera.orthographicSize = (isNegative ? -1 : 1) * CameraDimensions.y / (unitSize * 2f);
        }
    }
}
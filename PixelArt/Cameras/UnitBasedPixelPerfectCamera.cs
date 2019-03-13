using Exanite.PixelArt.Cameras.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt.Cameras
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
            int unitSize = MathE.GetNearestMultiple(Screen.height / VerticalUnits, Ppu);

            if (unitSize <= 0 || unitSize == int.MaxValue)
            {
                unitSize = Ppu;
            }

            _camera.orthographicSize = Screen.height / (unitSize * 2f);
        }
    }
}

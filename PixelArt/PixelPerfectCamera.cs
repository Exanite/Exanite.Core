using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class PixelPerfectCamera : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private int ppu = 16;
        [SerializeField, HideInInspector]
        private int verticalUnits = 4;
        private int screenHeight;

        private Camera _camera;

        [ShowInInspector]
        public int Ppu
        {
            get
            {
                return ppu;
            }

            set
            {
                if (value > 0)
                {
                    ppu = value;
                }

                CalculateCameraSize();
            }
        }

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

        private void Start()
        {
            _camera = GetComponent<Camera>();

            CalculateCameraSize();
        }

        private void Update()
        {
            if (screenHeight != Screen.height)
            {
                screenHeight = Screen.height;

                CalculateCameraSize();
            }
        }

        public void CalculateCameraSize()
        {
            int unitSize = MathE.GetNearestMultiple(Screen.height / VerticalUnits, Ppu);

            if(unitSize <= 0 || unitSize == int.MaxValue)
            {
                unitSize = Ppu;
            }

            _camera.orthographicSize = Screen.height / (unitSize * 2f);
        }
    } 
}
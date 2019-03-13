using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt.Cameras.Internal
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public abstract class PixelPerfectCameraBase : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private int ppu = 16;

        private int screenHeight;
        protected Camera _camera;

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

        protected virtual void Start()
        {
            _camera = GetComponent<Camera>();

            CalculateCameraSize();
        }

        protected virtual void Update()
        {
            if (screenHeight != Screen.height)
            {
                screenHeight = Screen.height;

                CalculateCameraSize();
            }
        }

        protected virtual void OnEnable()
        {
            if (_camera)
            {
                CalculateCameraSize();
            }
        }

        [Button(ButtonHeight = 25)]
        public abstract void CalculateCameraSize();
    }
}
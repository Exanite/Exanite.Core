using Exanite.Core.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.PixelArt.Cameras.Internal
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

        [InfoBox(message: "Camera Dimensions are odd, pixels may not render well.", InfoMessageType = InfoMessageType.Warning, VisibleIf = nameof(AreCameraDimensionsOdd))]
        [ShowInInspector]
        public Vector2Int CameraDimensions => new Vector2Int(_camera?.pixelWidth ?? 0, _camera?.pixelHeight ?? 0);

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

            if (_camera)
            {
                if (AreCameraDimensionsOdd())
                {
                    Debug.LogWarning($"Camera Dimensions are odd, pixels may not render well. Camera Dimensions: {CameraDimensions}");
                }
            }

            CalculateCameraSize();
        }

        protected virtual void Update()
        {
            if (_camera && screenHeight != CameraDimensions.y)
            {
                screenHeight = CameraDimensions.y;

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

        private bool AreCameraDimensionsOdd()
        {
            return CameraDimensions.x.IsOdd() || CameraDimensions.y.IsOdd();
        }
    }
}
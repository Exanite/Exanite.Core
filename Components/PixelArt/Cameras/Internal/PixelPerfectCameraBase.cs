using Exanite.Core.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras.Internal
{
    /// <summary>
    /// Abstract class for creating PixelPerfectCameras
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public abstract class PixelPerfectCameraBase : SerializedMonoBehaviour
    {
        [SerializeField, HideInInspector] private int pixelsPerUnit = 16;

        private int screenHeight;
        private Camera _camera;

        /// <summary>
        /// Current dimensions of the targeted <see cref="UnityEngine.Camera"/>
        /// </summary>
        [InfoBox(message: "Camera Dimensions are odd, pixels may not render well.", InfoMessageType = InfoMessageType.Warning, VisibleIf = nameof(AreCameraDimensionsOdd))]
        [ShowInInspector]
        public Vector2Int CameraDimensions => new Vector2Int(Camera?.pixelWidth ?? 0, Camera?.pixelHeight ?? 0);

        /// <summary>
        /// Pixels per unit to target
        /// </summary>
        [ShowInInspector]
        public int PixelsPerUnit
        {
            get
            {
                return pixelsPerUnit;
            }

            set
            {
                if (value > 0)
                {
                    pixelsPerUnit = value;
                }

                CalculateCameraSize();
            }
        }

        /// <summary>
        /// The targeted <see cref="UnityEngine.Camera"/>
        /// </summary>
        protected Camera Camera
        {
            get
            {
                if (!_camera)
                {
                    _camera = GetComponent<Camera>();
                }

                return _camera;
            }

            set
            {
                _camera = value;
            }
        }

        protected virtual void Start()
        {
            if (AreCameraDimensionsOdd())
            {
                Debug.LogWarning($"Camera Dimensions are odd, pixels may not render well. Camera Dimensions: {CameraDimensions}");
            }

            CalculateCameraSize();
        }

        protected virtual void Update()
        {
            if (Camera && screenHeight != CameraDimensions.y)
            {
                screenHeight = CameraDimensions.y;

                CalculateCameraSize();
            }
        }

        protected virtual void OnEnable()
        {
            if (Camera)
            {
                CalculateCameraSize();
            }
        }

        /// <summary>
        /// Calculates and sets the targeted <see cref="UnityEngine.Camera"/>'s orthographic size
        /// </summary>
        [Button(ButtonHeight = 25)]
        public abstract void CalculateCameraSize();

        private bool AreCameraDimensionsOdd()
        {
            return CameraDimensions.x.IsOdd() || CameraDimensions.y.IsOdd();
        }
    }
}
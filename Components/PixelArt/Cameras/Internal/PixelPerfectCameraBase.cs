﻿using Exanite.Core.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.Components.PixelArt.Cameras.Internal
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public abstract class PixelPerfectCameraBase : SerializedMonoBehaviour
    {
        [SerializeField, HideInInspector] private int pixelsPerUnit = 16;

        private int screenHeight;
        private Camera _camera;

        [InfoBox(message: "Camera Dimensions are odd, pixels may not render well.", InfoMessageType = InfoMessageType.Warning, VisibleIf = nameof(AreCameraDimensionsOdd))]
        [ShowInInspector]
        public Vector2Int CameraDimensions => new Vector2Int(Camera?.pixelWidth ?? 0, Camera?.pixelHeight ?? 0);

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

        [Button(ButtonHeight = 25)]
        public abstract void CalculateCameraSize();

        private bool AreCameraDimensionsOdd()
        {
            return CameraDimensions.x.IsOdd() || CameraDimensions.y.IsOdd();
        }
    }
}
using Exanite.Utility;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Exanite.PixelArt.Animation
{
    [RequireComponent(typeof(SpriteRenderer), typeof(PixelArtPositioner))]
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private new AnimationData animation;
        [SerializeField, HideInInspector]
        private float animationSpeed = 1;
        [SerializeField, HideInInspector]
        private int currentFrame;
        [SerializeField, HideInInspector]
        private bool useRootMotion = false;
        private float timeElapsed = 0;

        private SpriteRenderer _spriteRenderer;
        private PixelArtPositioner _pixelArtPositioner;

        [ShowInInspector]
        public AnimationData Animation
        {
            get
            {
                return animation;
            }

            set
            {
                if (value != animation)
                {
                    animation = value;
                    CurrentFrame = 0;
                    timeElapsed = 0;
                }
            }
        }

        [ShowInInspector]
        public float AnimationSpeed
        {
            get
            {
                return animationSpeed;
            }

            set
            {
                animationSpeed = value;
            }
        }

        [ShowInInspector]
        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }

            set
            {
                if (Animation == null || Animation.frames.IsNullOrEmpty())
                {
                    currentFrame = 0;

                    SpriteRenderer.sprite = null;
                }
                else
                {
                    currentFrame = value % Animation.Count;

                    if (currentFrame < 0)
                    {
                        currentFrame += Animation.Count;
                    }

                    SpriteRenderer.sprite = Animation[currentFrame].Sprite;
                    if (UseRootMotion)
                    {
                        PixelArtPositioner.PixelOffset += Animation[currentFrame].RootMotion;
                    }
                }
            }
        }

        [ShowInInspector]
        public bool UseRootMotion
        {
            get
            {
                return useRootMotion;
            }

            set
            {
                useRootMotion = value;
            }
        }

        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }

                return _spriteRenderer;
            }
        }

        public PixelArtPositioner PixelArtPositioner
        {
            get
            {
                if (_pixelArtPositioner == null)
                {
                    _pixelArtPositioner = GetComponent<PixelArtPositioner>();
                }

                return _pixelArtPositioner;
            }
        }

        private void Update()
        {
            if (Animation == null || Animation.frames.IsNullOrEmpty())
            {
                return;
            }

            timeElapsed += Time.deltaTime * AnimationSpeed;

            if (Math.Abs(timeElapsed) > Animation[CurrentFrame].Duration / 1000f)
            {
                CurrentFrame += (AnimationSpeed > 0) ? 1 : -1;
                timeElapsed = 0;
            }
        }
    } 
}
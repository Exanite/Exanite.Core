using Exanite.PixelArt;
using Exanite.Utility;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(PixelArtPositioner))]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private new AnimationData animation;
    [SerializeField, HideInInspector]
    private float animationSpeed = 1;
    [SerializeField, HideInInspector]
    private int currentFrame;
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
            animation = value;
            currentFrame = 0;
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
            }
            else
            {
                currentFrame = value % Animation.Count;

                if (currentFrame < 0)
                {
                    currentFrame += Animation.Count;
                }

                SpriteRenderer.sprite = Animation[currentFrame].Sprite;
                PixelArtPositioner.PixelOffset += Animation[currentFrame].RootMotion;
            }
        }
    }

    private SpriteRenderer SpriteRenderer
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

    private PixelArtPositioner PixelArtPositioner
    {
        get
        {
            if(_pixelArtPositioner == null)
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

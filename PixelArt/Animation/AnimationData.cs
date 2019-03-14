using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Project/AnimationData", order = 0)]
public class AnimationData : ScriptableObject
{
    public List<Frame> frames = new List<Frame>();

    public int Count
    {
        get
        {
            return frames.Count;
        }
    }

    public Frame this[int index]
    {
        get
        {
            return frames[index];
        }
    }

    [Serializable]
    public class Frame
    {
        [SerializeField, HideInInspector]
        private Sprite sprite;
        [SerializeField, HideInInspector]
        private int duration = 100;
        [SerializeField, HideInInspector]
        private Vector2 rootMotion = Vector2.zero;

        [ShowInInspector, PreviewField(Height = 100, Alignment = ObjectFieldAlignment.Right)]
        public Sprite Sprite
        {
            get
            {
                return sprite;
            }

            set
            {
                sprite = value;
            }
        }

        [ShowInInspector, HideLabel]
        public string Name
        {
            get
            {
                if (Sprite != null)
                {
                    return Sprite.name;
                }
                else
                {
                    return null;
                }
            }
        }

        [ShowInInspector, HorizontalGroup, PropertyTooltip("Duration in milliseconds")]
        public int Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        [ShowInInspector, HorizontalGroup]
        public Vector2 RootMotion
        {
            get
            {
                return rootMotion;
            }

            set
            {
                rootMotion = value;
            }
        }


    }
}
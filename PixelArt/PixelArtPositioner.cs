using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Core.PixelArt
{
    public class PixelArtPositioner : MonoBehaviour
    {
        public int ppu = 16;

        [ShowInInspector]
        public Vector2 PixelOffset
        {
            get
            {
                return transform.localPosition * ppu;
            }

            set
            {
                transform.localPosition = new Vector3(value.x / ppu, value.y / ppu, transform.localPosition.z);
            }
        }
    } 
}
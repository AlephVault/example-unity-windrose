using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            // Requiring Snapped, instead of Positionable, allows us to
            //   have the features of position automatically updated.
            //
            // We make no use of Snapped at all, but the behavior will
            //   automatically be called, and Positionable will be
            //   present anyway.
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(SpriteRenderer))]
            public class Sorted : MonoBehaviour
            {
                /**
                 * A represented object knows its z-order in the map.
                 */

                private SpriteRenderer spriteRenderer;
                private Positionable positionable;

                public enum SubLayer { LOW, MIDDLE, HIGH }
                [SerializeField]
                private SubLayer subLayer = SubLayer.MIDDLE;

                void Awake()
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    positionable = GetComponent<Positionable>();
                }

                void Update()
                {
                    // We order the sprite
                    uint h = positionable.ParentMap.Height;
                    uint w = positionable.ParentMap.Width;
                    int sortingOffset = (int)(w * h) * ((int)(subLayer));
                    spriteRenderer.sortingLayerID = 0;
                    spriteRenderer.sortingOrder = sortingOffset + (int)((h - 1 - positionable.Yf) * w + positionable.Xf);
                }

                void Pause(bool fullFreeze)
                {
                    enabled = false;
                }

                void Resume()
                {
                    enabled = true;
                }
            }
        }
    }
}
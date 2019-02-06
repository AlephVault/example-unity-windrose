using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            /// <summary>
            ///   The sub-layer 
            /// </summary>
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(SpriteRenderer))]
            public class Sorted : MonoBehaviour
            {
                private SpriteRenderer spriteRenderer;
                private Positionable positionable;

                /// <summary>
                ///   The sub-layer of an object in the map. Intended for
                ///     overlay/underlay effects of the objects, while
                ///     regular objects will be in the MIDDLE.
                /// </summary>
                public enum SubLayer { LOW, MIDDLE, HIGH }

                /// <summary>
                ///   The current sub-layer of the object. Usually, if this
                ///     sub-layer is chosen, it will be attached to another
                ///     object and use somehow a free movement strategy.
                /// </summary>
                /// <remarks>
                ///   Perhaps this property should NOT be here but inside the
                ///     inner status of the object inside a map.
                /// </remarks>
                [SerializeField]
                private SubLayer subLayer = SubLayer.MIDDLE;

                void Awake()
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    positionable = GetComponent<Positionable>();
                    positionable.onAttached.AddListener(delegate (World.Map map)
                    {
                        enabled = true;
                    });
                    positionable.onDetached.AddListener(delegate ()
                    {
                        enabled = false;
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the positionable. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Updates its sort order according to the X, Y, and Sub Layer of
                ///       this object.
                ///   </para>
                /// </summary>
                public void DoUpdate()
                {
                    // We order the sprite
                    uint h = positionable.ParentMap.Height;
                    uint w = positionable.ParentMap.Width;
                    int sortingOffset = (int)(w * h) * ((int)(subLayer));
                    spriteRenderer.sortingLayerID = 0;
                    spriteRenderer.sortingOrder = sortingOffset + (int)((h - 1 - positionable.Yf) * w + positionable.Xf);
                }
            }
        }
    }
}
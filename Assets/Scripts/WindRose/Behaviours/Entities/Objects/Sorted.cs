using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            /// <summary>
            ///   Sorts the element by its X position. The element is already sorted by its Y position.
            /// </summary>
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(SpriteRenderer))]
            public class Sorted : MonoBehaviour
            {
                private SpriteRenderer spriteRenderer;
                private Positionable positionable;

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
                    spriteRenderer.sortingLayerID = 0;
                    spriteRenderer.sortingOrder = (int)positionable.Xf;
                }
            }
        }
    }
}
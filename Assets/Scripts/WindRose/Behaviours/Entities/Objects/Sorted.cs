using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   Sorts the element by its X position. The element is already sorted by its Y position.
            /// </summary>
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(SpriteRenderer))]
            public class Sorted : MonoBehaviour
            {
                private SpriteRenderer spriteRenderer;
                private Object mapObject;

                void Awake()
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    mapObject = GetComponent<Object>();
                    mapObject.onAttached.AddListener(delegate (World.Map map)
                    {
                        enabled = true;
                    });
                    mapObject.onDetached.AddListener(delegate ()
                    {
                        enabled = false;
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the map object. It is
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
                    spriteRenderer.sortingOrder = (int)mapObject.Xf;
                }
            }
        }
    }
}
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            using World;

            /// <summary>
            ///   Handles the object's ability to animate, given sequences of sprites.
            ///     This one is the subclass for the Object, and will provide callbacks
            ///     like DoStart/DoUpdate.
            /// </summary>
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(Sorted))]
            public class Animated : Common.Animated
            {
                protected override void Awake()
                {
                    base.Awake();
                    Object mapObject = GetComponent<Object>();
                    mapObject.onAttached.AddListener(delegate (Map parentMap)
                    {
                        spriteRenderer.enabled = true;
                    });
                    mapObject.onDetached.AddListener(delegate ()
                    {
                        spriteRenderer.enabled = false;
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Start of the map object. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Initializes the default animation.
                ///   </para>
                /// </summary>
                public void DoStart()
                {
                    SetDefaultAnimation();
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the map object. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Updates the current animation frame on the object.
                ///   </para>
                /// </summary>
                public void DoUpdate()
                {
                    Frame();
                }
            }
        }
    }
}
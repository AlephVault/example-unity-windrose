using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            using World;

            /// <summary>
            ///   Handles the object's ability to animate, given sequences of sprites.
            /// </summary>
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(Sorted))]
            public class Animated : Visual.Animated
            {
                protected override void Awake()
                {
                    base.Awake();
                    Positionable positionable = GetComponent<Positionable>();
                    positionable.onAttached.AddListener(delegate (Map parentMap)
                    {
                        spriteRenderer.enabled = true;
                    });
                    positionable.onDetached.AddListener(delegate ()
                    {
                        spriteRenderer.enabled = false;
                    });
                }
            }
        }
    }
}
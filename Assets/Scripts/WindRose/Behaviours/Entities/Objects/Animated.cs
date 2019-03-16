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
            }
        }
    }
}